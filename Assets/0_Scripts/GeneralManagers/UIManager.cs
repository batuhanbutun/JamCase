using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button playButton,levelCompleteButton,levelRestartButton;
    [SerializeField] private GameObject menuPanel,gamePanel,victoryPanel,losePanel;
    [SerializeField] private List<TextMeshProUGUI> levelTexts;

    private void Start()
    {
        ButtonsInit();
        GameManager.Instance.OnGameSuccess += () => { victoryPanel.SetActive(true); };
        GameManager.Instance.OnGameFail += () => { losePanel.SetActive(true); };
        foreach (TextMeshProUGUI levelText in levelTexts)
            levelText.text = "Level " + (LevelManager.Instance.Level + 1);
        
    }

    private void ButtonsInit()
    {
        //Inspectorden atamayıp bağlılığı bir nebze azaltmak istedim. Daha büyük projelerde farklı yöntem izlenebilir bunlar çoğalabilir diye tabi.
        //Her paneli aynı interface veya abstractan kalıtım alıp daha sonrasında oyunun stateine göre işlemler yapılabilir. Şu anlık burda hallettim.
        playButton.onClick.AddListener(() =>
        {
            menuPanel.SetActive(false);
            gamePanel.SetActive(true);
        });
        levelCompleteButton.onClick.AddListener(() => { LevelManager.Instance.NextLevel(); });
        levelRestartButton.onClick.AddListener(() => { LevelManager.Instance.RestartLevel(); });
        
    }

}
