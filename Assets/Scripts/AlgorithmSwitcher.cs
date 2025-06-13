using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgorithmSwitcher : MonoBehaviour
{
    public BFSPathFinder bfs;
    public GreedyPathFinder greedy;
    public AStarPathFinder aStar;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            greedy.isSelected = false;
            aStar.isSelected = false;
            bfs.isSelected = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            bfs.isSelected = false;
            aStar.isSelected = false;
            greedy.isSelected = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            bfs.isSelected = false;
            greedy.isSelected = false;
            aStar.isSelected = true;
        }
    }
}
