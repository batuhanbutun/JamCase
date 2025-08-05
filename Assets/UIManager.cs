using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private GameObject menuPanel,victoryPanel,losePanel;
    

    private void Start()
    {
        playButton.onClick.AddListener(() =>
        {
            GameManager.Instance.StartGame();
            menuPanel.SetActive(false);
        });

        GameManager.Instance.OnGameSuccess += () =>
        {
            victoryPanel.SetActive(true);
        };
        
        GameManager.Instance.OnGameFail += () =>
        {
            losePanel.SetActive(true);
        };
    }

}
