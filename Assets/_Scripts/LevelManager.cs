using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private List<LevelData> levelDatas;
    [SerializeField] private GridManager3D gridManager;
    private LevelData currentLevelData;
    private int currentLevelIndex;
    private void Awake()
    {
        if(!PlayerPrefs.HasKey("LevelIndex"))
            PlayerPrefs.SetInt("LevelIndex", 0);
        
    }
    
    private void Start()
    {
        currentLevelIndex = PlayerPrefs.GetInt("LevelIndex");
        if(currentLevelIndex < levelDatas.Count)
            currentLevelData = levelDatas[currentLevelIndex];
        else
            currentLevelData = levelDatas[Random.Range(0, levelDatas.Count)];
        
        
        LoadLevel();
    }

    private void LoadLevel()
    {
        if (currentLevelData == null)
        {
            Debug.LogError("LevelData is not assigned!");
            return;
        }

        gridManager.GenerateGrid(currentLevelData.gridWidth, currentLevelData.gridHeight,currentLevelData.lockedGridPositions);

        foreach (var data in currentLevelData.passengerList)
        {
            gridManager.SpawnPassenger(data.gridPosition, data.color);
        }

        BusManager.Instance.SpawnInitialBuses(currentLevelData.busColorSequence);
    }
}
