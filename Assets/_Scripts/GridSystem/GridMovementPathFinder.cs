using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridMovementPathfinder
{
    public const float GRİD_SPACİNG = 0.5f;
    public static List<Vector2Int> GetPathToTop(Vector2Int start)
    {
        var gridPassengers = GridManager3D.Instance.gridPassengers;
        var width = GridManager3D.Instance.width;
        var height = GridManager3D.Instance.height;
        var lockedGrids = GridManager3D.Instance.lockedGrids;
        if (start.y == height - 1)
        {
            return new List<Vector2Int>(); 
        }
        
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(start);
        visited.Add(start);

        Vector2Int[] directions = {
            Vector2Int.up, Vector2Int.down,
            Vector2Int.left, Vector2Int.right
        };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current.y == height - 1 && gridPassengers[current.x, current.y] == null)
            {
                List<Vector2Int> path = new List<Vector2Int>();
                Vector2Int step = current;
                while (step != start)
                {
                    path.Add(step);
                    step = cameFrom[step];
                }
                path.Reverse();
                return path;
            }

            foreach (var dir in directions)
            {
                Vector2Int neighbor = current + dir;
                if (neighbor.x < 0 || neighbor.x >= width || neighbor.y < 0 || neighbor.y >= height)
                    continue;

                if (lockedGrids.Contains(neighbor)) continue;
                if (visited.Contains(neighbor)) continue;
                if (gridPassengers[neighbor.x, neighbor.y] != null) continue;

                visited.Add(neighbor);
                cameFrom[neighbor] = current;
                queue.Enqueue(neighbor);
            }
        }

        return null; // Ulaşılamazsa boş path döndür
    }

    public static Vector3[] GetPathWorldPoints(List<Vector2Int> gridPath)
    {
        Vector3[] worldPath = new Vector3[gridPath.Count];
        for (int i = 0; i < gridPath.Count; i++)
        {
            Vector3 pos = GetGridWorldPos(gridPath[i].x, gridPath[i].y);
            worldPath[i] = pos;
        }

        return worldPath;
    }
    
    public static Vector3 GetGridWorldPos(int x, int z)
    {
        float step = GRİD_SPACİNG;

        float totalHeight = (GridManager3D.Instance.height - 1) * step;
        float topZ = 5f; // burası senin oyunundaki sabit üst sınır
        float offsetZ = topZ - totalHeight;

        float offsetX = -(GridManager3D.Instance.width - 1) * step / 2f;

        return new Vector3(x * step + offsetX, 0f, z * step + offsetZ);
    }
    
    public static void EvaluatePassengerPaths()
    {
        if (GridManager3D.Instance.gridPassengers == null) return;
        for (int x = 0; x < GridManager3D.Instance.width; x++)
        {
            for (int z = 0; z < GridManager3D.Instance.height; z++)
            {
                Passenger p = GridManager3D.Instance.gridPassengers[x, z];
                if (p == null) continue;

                Vector2Int start = new(x, z);

                List<Vector2Int> path = GetPathToTop(start);

                bool hasPath = path != null;
                p.PassengerPathControl(hasPath);
            }
        }
    }
}