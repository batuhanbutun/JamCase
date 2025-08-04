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

    private Passenger[,] gridPassengers;
    private HashSet<Vector2Int> lockedGrids = new();

    public void GenerateGrid(int width1, int height1, List<Vector2Int> locked = null)
    {
        width = width1;
        height = height1;
        gridPassengers = new Passenger[width, height];

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
        lockedGrids = lockedSet;
    }

    public void TrySendToTop(int startX, int startZ)
    {
        Passenger passenger = gridPassengers[startX, startZ];
        if (passenger == null) return;
        
        Vector2Int start = new Vector2Int(startX, startZ);
        List<Vector2Int> path = GridMovementPathfinder.GetPathToTop(start, gridPassengers, width, height, lockedGrids);
        
        if (path != null)
            MoveAlongPath(passenger, startX, startZ, path);
        
    }

    private void MoveAlongPath(Passenger passenger, int startX, int startZ, List<Vector2Int> path)
    {
        gridPassengers[startX, startZ] = null;
        
        var passengerMovementController = passenger.GetComponent<PassengerMovementController>();
        passengerMovementController.MoveAlongGridPath(path, this, () =>
        {
            PassengerFlowController.Instance.RoutePassenger(passenger);
        });
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

        
        gridPassengers[gridPosition.x, gridPosition.y] = passenger;
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

}
