using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }

    [Header("Prefabs")] 
    [SerializeField] private Enemy standardZombiePrefab;
    [SerializeField] private Enemy standardPowerZombiePrefab;
    [SerializeField] private Enemy fatZombiePrefab;
    [SerializeField] private Enemy fatPowerZombiePrefab;
    [SerializeField] private Enemy fastZombiePrefab;
    [SerializeField] private Enemy fastPowerZombiePrefab;

    [Header("Pools")]
    [SerializeField] private Transform standardZombiePool;
    [SerializeField] private Transform standardPowerZombiePool;
    [SerializeField] private Transform fatZombiePool;
    [SerializeField] private Transform fatPowerZombiePool;
    [SerializeField] private Transform fastZombiePool;
    [SerializeField] private Transform fastPowerZombiePool;

    private Queue<Enemy> _standardZombies = new();
    private Queue<Enemy> _standardPowerZombies = new();
    private Queue<Enemy> _fatZombies = new();
    private Queue<Enemy> _fatPowerZombies = new();
    private Queue<Enemy> _fastZombies = new();
    private Queue<Enemy> _fastPowerZombies = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GeneratePools();
    }

    private void GeneratePools()
    {
        AddEnemy(100, ZombieType.STANDARD);
        AddEnemy(50, ZombieType.STANDARD, true);
        AddEnemy(100, ZombieType.FAT);
        AddEnemy(50, ZombieType.FAT, true);
        AddEnemy(100, ZombieType.FAST);
        AddEnemy(50, ZombieType.FAST, true);
    }

    private void AddEnemy(int count, ZombieType shellType, bool isPower = false)
    {
        switch (shellType)
        {
            case ZombieType.STANDARD:
                for (int i = 0; i < count; i++)
                {
                    if (isPower)
                    {
                        var enemy = Instantiate(standardPowerZombiePrefab, standardPowerZombiePool);
                        enemy.gameObject.SetActive(false);
                        _standardPowerZombies.Enqueue(enemy);
                    }
                    else
                    {
                        var enemy = Instantiate(standardZombiePrefab, standardZombiePool);
                        enemy.gameObject.SetActive(false);
                        _standardZombies.Enqueue(enemy);
                    }
                }
                break;
            case ZombieType.FAT:
                for (int i = 0; i < count; i++)
                {
                    if (isPower)
                    {
                        var enemy = Instantiate(fatPowerZombiePrefab, fatPowerZombiePool);
                        enemy.gameObject.SetActive(false);
                        _fatPowerZombies.Enqueue(enemy);
                    }
                    else
                    {
                        var enemy = Instantiate(fatZombiePrefab, fatZombiePool);
                        enemy.gameObject.SetActive(false);
                        _fatZombies.Enqueue(enemy);
                    }
                }
                break;
            case ZombieType.FAST:
                for (int i = 0; i < count; i++)
                {
                    if (isPower)
                    {
                        var enemy = Instantiate(fastPowerZombiePrefab, fastPowerZombiePool);
                        enemy.gameObject.SetActive(false);
                        _fastPowerZombies.Enqueue(enemy);
                    }
                    else
                    {
                        var enemy = Instantiate(fastZombiePrefab, fastZombiePool);
                        enemy.gameObject.SetActive(false);
                        _fastZombies.Enqueue(enemy);
                    }
                }
                break;
        }
    }

    public Enemy Get(ZombieType zombieType, bool isPower = false)
    {
        switch (zombieType)
        {
            case ZombieType.STANDARD:
                if (isPower)
                {
                    if (_standardPowerZombies.Count == 0) AddEnemy(1, zombieType, true);
                    return _standardPowerZombies.Dequeue();
                }
                else
                {
                    if (_standardZombies.Count == 0) AddEnemy(1, zombieType);
                    return _standardZombies.Dequeue();
                }
            case ZombieType.FAT:
                if (isPower)
                {
                    if (_fatPowerZombies.Count == 0) AddEnemy(1, zombieType, true);
                    return _fatPowerZombies.Dequeue();
                }
                else
                {
                    if (_fatZombies.Count == 0) AddEnemy(1, zombieType);
                    return _fatZombies.Dequeue();
                }
                
            case ZombieType.FAST:
                if (isPower)
                {
                    if (_fastPowerZombies.Count == 0) AddEnemy(1, zombieType, true);
                    return _fastPowerZombies.Dequeue();
                }
                else
                {
                    if (_fastZombies.Count == 0) AddEnemy(1, zombieType);
                    return _fastZombies.Dequeue();
                }
            
            default:
                if (_standardZombies.Count == 0) AddEnemy(1, zombieType);
                
                return _standardZombies.Dequeue();
        }
    }

    public void ReturnToPool(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);

        switch (enemy.ZombieType)
        {
            case ZombieType.STANDARD:
                if (enemy.IsPower)
                    _standardPowerZombies.Enqueue(enemy);
                else 
                    _standardZombies.Enqueue(enemy);
                break;
            case ZombieType.FAT:
                if (enemy.IsPower)
                    _fatPowerZombies.Enqueue(enemy);
                else 
                    _fatZombies.Enqueue(enemy);
                break;
            case ZombieType.FAST:
                if (enemy.IsPower)
                    _fastPowerZombies.Enqueue(enemy);
                else 
                    _fastZombies.Enqueue(enemy);
                break;
        }
    }
}