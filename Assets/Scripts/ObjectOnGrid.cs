using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class ObjectOnGrid : MonoBehaviour
{
    public GameObject obstaclesParent;

    public float obstacleProbability = 0.2f;

    public List<GameObject> obstacles = new List<GameObject>();
    List<GameObject> instObstacles = new List<GameObject>();

    public List<GameObject> animals = new List<GameObject>();
    List<GameObject> instAnimals = new List<GameObject>();

    public Material redMaterial;
    public Material blankMaterial;

    void Start()
    {
        SetupGrid();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGrid();
            SetupGrid();
        }
    }

    void SetupGrid()
    {
        NearestGrid others = GameObject.FindWithTag("Player").GetComponent<NearestGrid>();

        List<int> blankIndex = new List<int>();

        for (int i = 0; i < transform.childCount; i++)
        {
            float rand = Random.Range(0f, 1f);
            if (rand <= obstacleProbability)
            {
                int obstacleNum = Random.Range(0, 2);
                instObstacles.Add(Instantiate(obstacles[obstacleNum], transform.GetChild(i).position, Quaternion.identity, obstaclesParent.transform));

                transform.GetChild(i).GetComponent<BoxCollider>().isTrigger = false;
                transform.GetChild(i).GetComponent<NavMeshObstacle>().enabled = true;
                transform.GetChild(i).GetChild(4).GetComponent<MeshRenderer>().material = redMaterial;
            }
            else
            {
                others.canMoveGird.Add(i);
                blankIndex.Add(i);

                transform.GetChild(i).GetChild(4).GetComponent<MeshRenderer>().material = blankMaterial;
            }
        }

        for (int i = 0; i < animals.Count; i++)
        {
            int randomIndex = Random.Range(0, blankIndex.Count);

            GameObject temp = Instantiate(animals[i], transform.GetChild(blankIndex[randomIndex]).position, Quaternion.identity);
            instAnimals.Add(temp);
            temp.GetComponent<NearestGrid>().canMoveGird = others.canMoveGird;

            blankIndex.RemoveAt(randomIndex);
        }
    }

    void ResetGrid()
    {
        for (int i = 0; i < instAnimals.Count; i++)
        {
            if (instAnimals[i] != null)
                Destroy(instAnimals[i]);
        }
        instAnimals.Clear();

        foreach (GameObject obj in instObstacles)
        {
            Destroy(obj);
        }
        instObstacles.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<BoxCollider>().isTrigger = true;
            transform.GetChild(i).GetComponent<NavMeshObstacle>().enabled = false;
            transform.GetChild(i).GetChild(4).GetComponent<MeshRenderer>().material = blankMaterial;
        }
    }
}
