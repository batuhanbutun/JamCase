using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum GameState{MENU,PLAY,EOL}
public class GameManager : Singleton<GameManager>
{
    public System.Action OnGameStart,OnGameSuccess,OnGameFail;
    public GameState gameState = GameState.MENU;

    private void Start()
    {
        CountdownTimer.OnTimerEnd += GameFail;
    }

    public void StartGame()
    {
        OnGameStart?.Invoke();
        gameState = GameState.PLAY;
    }

    public void GameWin()
    {
        if(gameState == GameState.EOL) return;
        OnGameSuccess?.Invoke();
        gameState = GameState.EOL;
    }
    
    public void GameFail()
    {
        if(gameState == GameState.EOL) return;
        OnGameFail?.Invoke();
        gameState = GameState.EOL;
        DOTween.KillAll();
    }
    
}
