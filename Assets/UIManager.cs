using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button playButton,levelCompleteButton,levelRestartButton;
    [SerializeField] private GameObject menuPanel,gamePanel,victoryPanel,losePanel;
    

    private void Start()
    {
        ButtonsInit();

        GameManager.Instance.OnGameSuccess += () =>
        {
            victoryPanel.SetActive(true);
        };
        
        GameManager.Instance.OnGameFail += () =>
        {
            losePanel.SetActive(true);
        };
    }

    private void ButtonsInit()
    {
        //Inspectorden atamayıp bağlılığı bir nebze azaltmak istedim. Daha büyük çaplı projelerde direkt dependency injection tercih edilebilir bağlılıklarda tabii.
        playButton.onClick.AddListener(() =>
        {
            GameManager.Instance.StartGame();
            menuPanel.SetActive(false);
            gamePanel.SetActive(true);
        });
        levelCompleteButton.onClick.AddListener(() => { LevelManager.Instance.NextLevel(); });
        levelRestartButton.onClick.AddListener(() => { LevelManager.Instance.RestartLevel(); });
        
    }

}
