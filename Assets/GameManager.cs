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
    public void StartGame()
    {
        OnGameStart?.Invoke();
        gameState = GameState.PLAY;
    }

    public void GameWin()
    {
        OnGameSuccess?.Invoke();
    }
    
    public void GameFail()
    {
        OnGameFail?.Invoke();
        gameState = GameState.EOL;
        DOTween.KillAll();
    }
    
}
