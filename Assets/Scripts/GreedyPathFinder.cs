using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GreedyPathFinder : MonoBehaviour
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

            GreedyFind(new Vector2Int(nearestGrid.nearGridIndex / 11, nearestGrid.nearGridIndex % 11), new Vector2Int(playerNearestGrid.nearGridIndex / 11, playerNearestGrid.nearGridIndex % 11));

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

    public Material pathMat;
    public Material unselectedPathMat;
    public Material blankMat;

    List<Vector2Int> path = new();

    public void GreedyFind(Vector2Int start, Vector2Int goal)
    {
        PriorityQueue<Vector2Int> pq = new PriorityQueue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> parent = new();
        HashSet<Vector2Int> visited = new();

        pq.Enqueue(start, Heuristic(start, goal));
        visited.Add(start);

        Vector2Int[] directions = {
            new(1, 0), new(-1, 0),
            new(0, 1), new(0, -1)
        };

        while (pq.Count > 0)
        {
            Vector2Int current = pq.Dequeue();

            if (current == goal)
                break;

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;

                if (InBounds(next) && grids[next.x, next.y] && !visited.Contains(next))
                {
                    float priority = Heuristic(next, goal);
                    pq.Enqueue(next, priority);
                    visited.Add(next);
                    parent[next] = current;
                }
            }
        }

        if (!parent.ContainsKey(goal))
        {
            Debug.LogWarning("경로를 찾을 수 없습니다.");
            return;
        }

        if (path.Count > 0)
            RestPathMat(path);

        path = new();
        Vector2Int curr = goal;
        while (curr != start)
        {
            path.Add(curr);
            curr = parent[curr];
        }

        path.Reverse();
        if (!isSelected)
            VisualizePath(path);
    }

    float Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    bool InBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

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