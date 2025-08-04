using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridMovementPathfinder
{
    public static List<Vector2Int> GetPathToTop(Vector2Int start, GameObject[,] gridObjects, int width, int height, HashSet<Vector2Int> lockedGrids)
    {
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

            if (current.y == height - 1 && gridObjects[current.x, current.y] == null)
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
                if (gridObjects[neighbor.x, neighbor.y] != null) continue;

                visited.Add(neighbor);
                cameFrom[neighbor] = current;
                queue.Enqueue(neighbor);
            }
        }

        return null; // Ulaşılamazsa boş path döndür
    }
}