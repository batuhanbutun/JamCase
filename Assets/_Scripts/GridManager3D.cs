using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GridManager3D : MonoBehaviour
{
    public int width = 6;
    public int height = 10;
    public float cellSize = 1.1f;

    public GameObject cubePrefab;
    public GameObject gridTilePrefab;

    private GameObject[,] gridObjects;

    private void Start()
    {
        //GenerateGrid();
    }

    public void GenerateGrid(int width, int height)
    {
        gridObjects = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // Grid zeminini oluştur
                Vector3 tilePos = GetWorldPos(x, z);
                Instantiate(gridTilePrefab, tilePos, Quaternion.identity, transform).name = $"Tile_{x}_{z}";

                // %50 ihtimalle üzerine blok koy
                if (Random.value > 0.5f)
                {
                    Vector3 blockPos = tilePos + Vector3.up * 0.5f;
                    GameObject obj = Instantiate(cubePrefab, blockPos, Quaternion.identity, transform);
                    obj.name = $"Block_{x}_{z}";
                    obj.AddComponent<BoxCollider>();

                    var click = obj.AddComponent<BlockClick3D>();
                    click.Init(x, z, this);

                    gridObjects[x, z] = obj;
                }
            }
        }
    }

    public void TrySendToTop(int startX, int startZ)
    {
        GameObject obj = gridObjects[startX, startZ];
        if (obj == null) return;

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        Vector2Int start = new Vector2Int(startX, startZ);
        queue.Enqueue(start);
        visited.Add(start);

        Vector2Int[] directions = {
            Vector2Int.up, Vector2Int.down,
            Vector2Int.left, Vector2Int.right
        };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            // En üst satırda boş bir yer varsa hedefimiz budur
            if (current.y == height - 1 && gridObjects[current.x, current.y] == null)
            {
                // Yolu çöz
                List<Vector2Int> path = new List<Vector2Int>();
                Vector2Int step = current;
                while (step != start)
                {
                    path.Add(step);
                    step = cameFrom[step];
                }
                path.Reverse();

                StartCoroutine(MoveAlongPath(obj, startX, startZ, path));
                return;
            }

            foreach (var dir in directions)
            {
                Vector2Int neighbor = current + dir;
                if (neighbor.x < 0 || neighbor.x >= width || neighbor.y < 0 || neighbor.y >= height)
                    continue;

                if (visited.Contains(neighbor)) continue;
                if (gridObjects[neighbor.x, neighbor.y] != null) continue;

                visited.Add(neighbor);
                cameFrom[neighbor] = current;
                queue.Enqueue(neighbor);
            }
        }
    }

    IEnumerator MoveAlongPath(GameObject obj, int startX, int startZ, List<Vector2Int> path)
    {
        gridObjects[startX, startZ] = null;

        // World pozisyon listesini hazırla
        Vector3[] worldPath = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 pos = GetWorldPos(path[i].x, path[i].y) + Vector3.up * 0.5f;
            worldPath[i] = pos;
        }

        // Son pozisyona grid'de kaydet
        Vector2Int last = path[^1];
        //gridObjects[last.x, last.y] = obj;

        float duration = path.Count * 0.2f; // toplam süre

        // Hareket başlasın
        yield return obj.transform.DOPath(worldPath, duration, PathType.Linear)
            .SetEase(Ease.InOutSine)
            .WaitForCompletion();
        PassengerRouter(obj.GetComponent<Passenger>());
    }
    
    private void PassengerRouter(Passenger passenger)
    {
        if (BusManager.Instance.TryReceive(passenger)) return;
        if (WaitingAreaManager.Instance.TryReceive(passenger)) return;

        // ikisi de alamadı: game over
        Debug.Log("Game Over!");
    }

    public void SpawnPassenger()
    {
        
    }

    public Vector3 GetWorldPos(int x, int z)
    {
        float bottomZ = 5 - (height - 1) * cellSize;
        float worldZ = bottomZ + z * cellSize;

        return new Vector3(x * cellSize, 0f, worldZ);
    }
}
