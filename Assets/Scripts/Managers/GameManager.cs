using System;

public static class GameManager
{
    public static event Action<GameState> OnGameStateChange;
    public static event Action OnLevelRestart;
    
    private static int _lives = 1;
    
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
        _lives = 1;
        OnLevelRestart?.Invoke();
    }

    public static void FinishLevel()
    {
        
    }
    
    public static bool HasLives => _lives > 0;
    
    public static void RemoveLive()
    {
        _lives--;
    }
}
