using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    
    private float startingTime;
    private float timeRemaining;
    private bool isCounting = false;

    public static System.Action OnTimerEnd;
    private void Start()
    {
        GameManager.Instance.OnGameStart += StartCountdownTimer;
        GameManager.Instance.OnGameSuccess += CloseTimer;
    }

    private void Update()
    {
        if (!isCounting) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            TimerEnd();
            isCounting = false;
        }
        UpdateTimer();
    }

    private void StartCountdownTimer()
    {
        timerText.gameObject.SetActive(true);
        startingTime = LevelManager.Instance.currentLevelData.levelDuration;
        timeRemaining = startingTime;
        isCounting = true;
        UpdateTimer(); 
    }
    
    private void CloseTimer()
    {
        isCounting = false;
        timerText.gameObject.SetActive(false);
    }
    
    private void UpdateTimer()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);
        
        timerText.text = formattedTime;
    }

    private void TimerEnd()
    {
        OnTimerEnd?.Invoke();
    }
}
