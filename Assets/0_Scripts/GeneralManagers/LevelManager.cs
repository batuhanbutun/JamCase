using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private List<LevelData> levelDatas;
    public LevelData currentLevelData;
    private int currentLevelIndex;

    public int Level => currentLevelIndex;
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

        var gridManager = GridManager3D.Instance;
        gridManager.GenerateGrid(currentLevelData.gridWidth, currentLevelData.gridHeight,currentLevelData.lockedGridPositions);

        foreach (var data in currentLevelData.passengerList)
        {
            gridManager.SpawnPassenger(data.gridPosition, data.color,data.passengerType);
        }

        BusManager.Instance.SpawnInitialBuses(currentLevelData.busColorSequence);
    }

    public void NextLevel()
    {
        PlayerPrefs.SetInt("LevelIndex", currentLevelIndex + 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameManager.Instance.RestartLevel();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameManager.Instance.RestartLevel();
    }
}
