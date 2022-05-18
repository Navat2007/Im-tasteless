using System;

[Serializable]
public class Wave
{
    public bool active;
    public bool done;
    public WaveSO waveSO;

    private float _nextSpawnTime;
    private int _currentFatCount;
    private int _currentFastCount;
    private int _enemyRemainingToSpawn;
    private int _enemiesRemainingAlive;
    private int _enemiesAlreadySpawned;

    public float GetNextSpawnTime => _nextSpawnTime;
    public void SetNextSpawnTime (float value) => _nextSpawnTime = value;

    public int GetRemainingToSpawn => _enemyRemainingToSpawn;
    public void SetRemainingToSpawn (int value) => _enemyRemainingToSpawn = value;
        
    public int GetRemainingAlive => _enemiesRemainingAlive;
    public void SetRemainingAlive (int value) => _enemiesRemainingAlive = value;
        
    public int GetAlreadySpawned => _enemiesAlreadySpawned;
    public void SetAlreadySpawned (int value) => _enemiesAlreadySpawned = value;
        
    public int GetFatCount => _currentFatCount;
    public void SetFatCount (int value) => _currentFatCount = value;
        
    public int GetFastCount => _currentFastCount;
    public void SetFastCount (int value) => _currentFastCount = value;
}
