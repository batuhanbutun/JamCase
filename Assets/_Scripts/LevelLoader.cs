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

        gridManager.GenerateGrid(levelData.gridWidth, levelData.gridHeight,levelData.lockedGridPositions);

        foreach (var data in levelData.passengerList)
        {
            gridManager.SpawnPassenger(data.gridPosition, data.color);
        }

        BusManager.Instance.SpawnInitialBuses(levelData.busColorSequence);
        WaitingAreaManager.Instance.ResetArea();
    }
}
