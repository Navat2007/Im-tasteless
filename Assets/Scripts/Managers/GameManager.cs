using System;

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
}
