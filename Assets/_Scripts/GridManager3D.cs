using System.Collections;
using System.Collections.Generic;
using _Scripts;
using DG.Tweening;
using UnityEngine;

public class GridManager3D : MonoBehaviour
{
    public int width = 6;
    public int height = 10;
    public float cellSize = 1.1f;
    public float gridSpacing = 0.5f;

    public Passenger passengerPrefab;
    public GameObject gridTilePrefab;

    private GameObject[,] gridObjects;
    private HashSet<Vector2Int> lockedGrids = new();

    public void GenerateGrid(int width1, int height1, List<Vector2Int> locked = null)
    {
        width = width1;
        height = height1;
        gridObjects = new GameObject[width, height];

        HashSet<Vector2Int> lockedSet = locked != null ? new HashSet<Vector2Int>(locked) : new();

        
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector2Int gridPos = new(x, z);
                if (lockedSet.Contains(gridPos))
                    continue;
                
                Vector3 tilePos = GetWorldPos(x, z);
                Instantiate(gridTilePrefab, tilePos, Quaternion.identity, transform).name = $"Tile_{x}_{z}";
            }
        }
        this.lockedGrids = lockedSet;
    }

    public void TrySendToTop(int startX, int startZ)
    {
        //GridMovementPathfinder.GetPathToTop();
        GameObject obj = gridObjects[startX, startZ];
        if (obj == null) return;

        Vector2Int start = new Vector2Int(startX, startZ);

        List<Vector2Int> path = GridMovementPathfinder.GetPathToTop(
            start, gridObjects, width, height, lockedGrids
        );

        if (path != null)
        {
            StartCoroutine(MoveAlongPath(obj, startX, startZ, path));
        }
    }

    IEnumerator MoveAlongPath(GameObject obj, int startX, int startZ, List<Vector2Int> path)
    {
        gridObjects[startX, startZ] = null;

        // World pozisyon listesini hazırla
        Vector3[] worldPath = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 pos = GetWorldPos(path[i].x, path[i].y);
            worldPath[i] = pos;
        }

        // Son pozisyona grid'de kaydet
        Vector2Int last = path[^1];
        //gridObjects[last.x, last.y] = obj;

        // Hareket başlasın
        yield return StartCoroutine(obj.GetComponent<IMovable>().FollowPath(worldPath, () =>
        {
            PassengerRouter(obj.GetComponent<Passenger>()); 
        }));
    }
    
    private void PassengerRouter(Passenger passenger)
    {
        if (BusManager.Instance.TryReceive(passenger)) return;
        if (WaitingAreaManager.Instance.TryReceive(passenger)) return;

        // ikisi de alamadı: game over
        Debug.Log("Game Over!");
    }

    public void SpawnPassenger(Vector2Int gridPosition,ObjColor passengerColor)
    {
        Vector3 tilePos = GetWorldPos(gridPosition.x, gridPosition.y);
        Vector3 passengerPos = tilePos ;
        Passenger passenger = Instantiate(passengerPrefab, passengerPos, Quaternion.identity, transform);
        passenger.name = $"Passenger_{gridPosition.x}_{gridPosition.y}";
        passenger.gameObject.AddComponent<BoxCollider>();

        var click = passenger.gameObject.AddComponent<BlockClick3D>();
        click.Init(gridPosition.x, gridPosition.y, this);

        
        gridObjects[gridPosition.x, gridPosition.y] = passenger.gameObject;
        passenger.ColorSetup(passengerColor);
    }

    public Vector3 GetWorldPos(int x, int z)
    {
       float step = gridSpacing;

       float totalHeight = (height - 1) * step;
       float topZ = 5f; // burası senin oyunundaki sabit üst sınır
       float offsetZ = topZ - totalHeight;

       float offsetX = -(width - 1) * step / 2f;

       return new Vector3(x * step + offsetX, 0f, z * step + offsetZ);
       
    }
    
    private bool IsValidGridPos(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height)
            return false;

        if (lockedGrids != null && lockedGrids.Contains(pos))
            return false;

        return true;
    }
}
