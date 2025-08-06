using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts;
using DG.Tweening;
using UnityEngine;

public class GridManager3D : Singleton<GridManager3D>
{
    public int width = 6;
    public int height = 10;

    public Passenger passengerPrefab;
    public GameObject gridTilePrefab;

    public Passenger[,] gridPassengers;
    public HashSet<Vector2Int> lockedGrids = new();
    
    [SerializeField] private List<PassengerTypeAndEnums> passengerTypeAndEnums;

    
    private void Start()
    {
        GameManager.Instance.OnGameStart += GridMovementPathfinder.EvaluatePassengerPaths;
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
                
                Vector3 tilePos = GridMovementPathfinder.GetGridWorldPos(x, z);
                Instantiate(gridTilePrefab, tilePos, Quaternion.identity, transform).name = $"Tile_{x}_{z}";
            }
        }
        lockedGrids = lockedSet;
    }
    
    public void SpawnPassenger(Vector2Int gridPosition,ObjColor passengerColor,PassengerType passengerType)
    {
        Vector3 tilePos = GridMovementPathfinder.GetGridWorldPos(gridPosition.x, gridPosition.y);
        Vector3 passengerPos = tilePos ;
        var toSpawnedPassengerPrefab = passengerTypeAndEnums.FirstOrDefault(x=>x.passengerType == passengerType)?.passengerPrefab.GetComponent<Passenger>();
        Passenger passenger = Instantiate(toSpawnedPassengerPrefab, passengerPos, Quaternion.identity, transform);
        passenger.name = $"Passenger_{gridPosition.x}_{gridPosition.y}";

        var click = passenger.gameObject.AddComponent<PassengerClick>();
        click.Init(gridPosition.x, gridPosition.y, this);

        
        gridPassengers[gridPosition.x, gridPosition.y] = passenger;
        passenger.ColorSetup(passengerColor);
    }
    

}
