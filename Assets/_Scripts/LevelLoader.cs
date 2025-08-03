using System.Collections;
using System.Collections.Generic;
using _Scripts;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private LevelData levelData;
    [SerializeField] private GridManager3D gridManager;
    [SerializeField] private Passenger passengerPrefab;
    
    private void Start()
    {
        LoadLevel();
    }

    private void LoadLevel()
    {
        if (levelData == null)
        {
            Debug.LogError("LevelData is not assigned!");
            return;
        }

        gridManager.GenerateGrid(levelData.gridWidth, levelData.gridHeight);

        foreach (var data in levelData.passengerList)
        {
            SpawnPassenger(data.gridPosition, data.color);
        }

        //BusManager.Instance.SetupBuses(levelData.busColorSequence);
    }

    private void SpawnPassenger(Vector2Int gridPos, ObjColor color)
    {
        Vector3 worldPos = gridManager.GetWorldPos(gridPos.x, gridPos.y);

        var passenger = Instantiate(passengerPrefab, worldPos, Quaternion.identity);
        passenger.ColorSetup(color);

        //gridManager.RegisterPassenger(gridPos, passenger);
    }
}
