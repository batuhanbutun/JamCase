using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using DG.Tweening;
using UnityEngine;

public class GridManager3D : Singleton<GridManager3D>
{
    public int width = 6;
    public int height = 10;
    public float gridSpacing = 0.5f;

    public Passenger passengerPrefab;
    public GameObject gridTilePrefab;

    private Passenger[,] gridPassengers;
    private HashSet<Vector2Int> lockedGrids = new();
    
    
    private void Start()
    {
        GameManager.Instance.OnGameStart += () =>
        {
            EvaluatePassengerPaths((passenger1, hasPath) =>
            {
                passenger1.PassengerPathControl(hasPath);
            });
        };
    }

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
        var busAvailable = BusManager.Instance.IsCurrentBusAvailable(passenger);
        var waitingAreaAvailable = WaitingAreaManager.Instance.IsAvailable();
        if(!waitingAreaAvailable && !busAvailable) return;
        
        gridPassengers[startX, startZ] = null;
        EvaluatePassengerPaths((passenger1, hasPath) =>
        {
            passenger1.PassengerPathControl(hasPath); 
        });
        var passengerMovementController = passenger.GetComponent<PassengerMovementController>();
        passenger.GetComponent<IOutlinable>().OutlineSet(false);
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
    
    private void EvaluatePassengerPaths(System.Action<Passenger, bool> callback)
    {
        if (gridPassengers == null) return;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Passenger p = gridPassengers[x, z];
                if (p == null) continue;

                Vector2Int start = new(x, z);

                List<Vector2Int> path = GridMovementPathfinder.GetPathToTop(
                    start,
                    gridPassengers,
                    width,
                    height,
                    lockedGrids
                );

                bool hasPath = path != null && path.Count > 0;
                callback?.Invoke(p, hasPath);
            }
        }
    }

}
