using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BFSPathFinder : MonoBehaviour
{
    GameObject grid;

    Animal animal;

    FoodInventory inventory;

    NearestGrid playerNearestGrid;
    NearestGrid nearestGrid;

    bool[,] grids = new bool[11, 11];

    public bool isSelected = false;
    bool check = false;

    void Start()
    {
        grid = GameObject.FindFirstObjectByType<ObjectOnGrid>().gameObject;
        animal = GetComponent<Animal>();
        inventory = GameObject.FindFirstObjectByType<FoodInventory>();
        playerNearestGrid = GameObject.FindWithTag("Player").GetComponent<NearestGrid>();
        nearestGrid = GetComponent<NearestGrid>();

        SetArray();
    }

    void Update()
    {
        if ((inventory.index + 1) == (int)animal.needFood)
        {
            check = true;

            BFSFind(new Vector2Int(nearestGrid.nearGridIndex / 11, nearestGrid.nearGridIndex % 11), new Vector2Int(playerNearestGrid.nearGridIndex / 11, playerNearestGrid.nearGridIndex % 11));

            if (!ArePathsEqual(animal.path, path) && isSelected)
            {
                animal.SetPath(path);
            }
        }
        else
        {
            if (path.Count > 0)
            {
                RestPathMat(path);
                path.Clear();
            }
            
            if (check)
            {
                if (isSelected)
                    animal.StartRandomMove();

                check = false;
            }
        }
    }

    bool ArePathsEqual(List<Vector2Int> a, List<Vector2Int> b)
    {
        if (a == null || b == null) return false;
        if (a.Count != b.Count) return false;

        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i])
                return false;
        }
        return true;
    }

    private void LateUpdate()
    {
        if (isSelected && path.Count > 0)
            VisualizePath(path);
    }

    void SetArray()
    {
        grids = new bool[11, 11];
        for (int i = 0; i < nearestGrid.canMoveGird.Count; i++)
        {
            grids[nearestGrid.canMoveGird[i] / 11, nearestGrid.canMoveGird[i] % 11] = true;
        }
    }

    int width = 11;
    int height = 11;
    int[,] visited = new int[11, 11];

    Queue<Vector2Int> queue = new Queue<Vector2Int>();
    Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();
    List<Vector2Int> path = new List<Vector2Int>();

    public void BFSFind(Vector2Int start, Vector2Int goal)
    {
        visited = new int[width, height];
        queue.Clear();
        parent.Clear();

        queue.Enqueue(start);
        visited[start.x, start.y] = 1;

        Vector2Int[] directions = {
        new Vector2Int(1, 0), new Vector2Int(-1, 0),
        new Vector2Int(0, 1), new Vector2Int(0, -1)
    };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == goal)
                break;

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;

                if (next.x >= 0 && next.x < width && next.y >= 0 && next.y < height)
                {
                    if (visited[next.x, next.y] == 0 && grids[next.x, next.y])
                    {
                        queue.Enqueue(next);
                        visited[next.x, next.y] = 1;
                        parent[next] = current;
                    }
                }
            }
        }

        if (path.Count > 0)
            RestPathMat(path);

        path = new List<Vector2Int>();
        Vector2Int curr = goal;

        while (curr != start)
        {
            if (!parent.ContainsKey(curr))
            {
                Debug.LogWarning("경로를 찾을 수 없습니다.");
                return;
            }

            path.Add(curr);
            curr = parent[curr];
        }

        path.Reverse();
        if (!isSelected)
            VisualizePath(path);
    }

    void DebugArray()
    {
        for (int i = 0; i < grids.GetLength(0); i++)
        {
            string row = "";
            for (int j = 0; j < grids.GetLength(1); j++)
            {
                if (grids[i, j])
                    row += $"{"1",4}";
                else
                    row += $"{"0",4}";
            }
            Debug.Log(row);
        }
    }

    public Material pathMat;
    public Material unselectedPathMat;
    public Material blankMat;

    void VisualizePath(List<Vector2Int> path)
    {
        path.RemoveAt(path.Count - 1);

        foreach (Vector2Int vec in path)
        {
            int index = vec.x * 11 + vec.y;

            MeshRenderer mesh = grid.transform.GetChild(index).GetChild(4).GetComponent<MeshRenderer>();
            mesh.material = isSelected ? pathMat : unselectedPathMat;
        }
    }

    void RestPathMat(List<Vector2Int> path)
    {
        foreach (Vector2Int vec in path)
        {
            int index = vec.x * 11 + vec.y;

            grid.transform.GetChild(index).GetChild(4).GetComponent<MeshRenderer>().material = blankMat;
        }
    }
}
