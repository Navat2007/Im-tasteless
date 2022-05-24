using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct WaveStruct
{
    [Header("Параметры волны")]
    public bool infinite;
    public int enemyCount;
    public float timeBetweenSpawn ;
        
    [Header("Варианты врагов")]
    public List<ZombieType> enemyList;
    [Space]
    public float powerEnemyChance;
    public int fatMaxCount;
    public int fastMaxCount;

    [Header("Дополнительная прибавка к параметрам каждого врага")]
    public int xpOnDeath;
    public float damage;
    public float moveSpeed;
    public float health;
}

[Serializable]
public class Wave
{
    public bool active;
    public bool done;
    public bool finalWave;
    public WaveStruct waveStruct;
    public List<Enemy> enemies = new();

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

    public void AddEnemyToList(Enemy enemy)
    {
        enemies.Add(enemy);
    }
    
    public void RemoveEnemyFromList(Enemy enemy)
    {
        enemies.Remove(enemy);
    }
}