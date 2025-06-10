using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearestGrid : MonoBehaviour
{
    public GameObject grid;

    public List<int> canMoveGird = new List<int>();

    public Material nearMat;
    public Material blankMat;

    string instNearMatName;
    string instBlankMatName;

    int prevIndex = -1;

    void Start()
    {
        if (grid == null)
            grid = GameObject.FindFirstObjectByType<ObjectOnGrid>().gameObject;

        instNearMatName = nearMat.name + " (Instance)";
        instBlankMatName = blankMat.name + " (Instance)";
    }

    void Update()
    {
        int nearGridIndex = -1;
        float minDist = float.MaxValue;

        foreach (int v in canMoveGird)
        {
            float dist = Vector3.Distance(grid.transform.GetChild(v).position, transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearGridIndex = v;
            }
        }

        if (prevIndex > -1 && prevIndex != nearGridIndex)
        {
            MeshRenderer prevMeshRender = grid.transform.GetChild(prevIndex).GetChild(4).GetComponent<MeshRenderer>();
            if (prevMeshRender.material.name == instNearMatName)
                prevMeshRender.material = blankMat;
        }

        MeshRenderer meshRenderer = grid.transform.GetChild(nearGridIndex).GetChild(4).GetComponent<MeshRenderer>();
        if (meshRenderer.material.name == instBlankMatName)
        {
            prevIndex = nearGridIndex;
            meshRenderer.material = nearMat;
        }
    }

    void OnDestroy()
    {
        if (prevIndex > -1)
        {
            MeshRenderer prevMeshRender = grid.transform.GetChild(prevIndex).GetChild(4).GetComponent<MeshRenderer>();
            if (prevMeshRender.material.name == instNearMatName)
                prevMeshRender.material = blankMat;
        }
    }
}
