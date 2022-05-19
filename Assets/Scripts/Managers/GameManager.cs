using System;
using UnityEngine;

public static class GameManager
{
    public static event Action<GameState> OnGameStateChange;
    public static event Action OnLevelRestart;
    
    public enum GameState
    {
        PLAY, 
        PAUSE
    }

    private static GameState _currentGameState = GameState.PLAY;

    public static GameState GetGameState()
    {
        return _currentGameState;
    }
    
    public static void SetGameState(GameState state)
    {
        _currentGameState = state;
        OnGameStateChange?.Invoke(_currentGameState);
    }

    public static void LevelRestart()
    {
        OnLevelRestart?.Invoke();
    }

    public static void FinishLevel()
    {
        
    }
    
    public static void StartPause()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        SetGameState(GameState.PAUSE);
    }
    
    public static void StopPause()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        SetGameState(GameState.PLAY); 
    }
}
