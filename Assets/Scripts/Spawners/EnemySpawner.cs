using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class EnemySpawner : MonoBehaviour
{
    [Header("Точки спавна на карте")]
    [SerializeField] private List<SpawnPoint> spawnPoints = new();

    [Header("Настройка волн")]
    [SerializeField] private int _maxActiveEnemy = 40;
    [SerializeField] private List<Wave> waves = new();

    [Header("Стандартный враг, если не найдены остальные")]
    [SerializeField] private Enemy standardEnemyPrefab;

    [Space(10)]
    [Header("Дистанция спавна")]
    [SerializeField] private float minSpawnDistance = -5;

    [SerializeField] private float maxSpawnDistance = 5;

    [Header("Хранилище объектов")]
    [SerializeField] private List<Enemy> enemyList;

    [Header("Шанс спавна бустеров")]
    [SerializeField] private float standardBusterSpawnChance = 5.0f;

    [SerializeField] private float powerStandardBusterSpawnChance = 50.0f;
    [SerializeField] private float fastBusterSpawnChance = 10.0f;
    [SerializeField] private float powerFastBusterSpawnChance = 25.0f;
    [SerializeField] private float fatBusterSpawnChance = 15.0f;
    [SerializeField] private float powerFatBusterSpawnChance = 50.0f;

    private float _bonusPowerEnemySpawnChance;
    private Wave _currentWave;
    private int _currentWaveIndex = -1;

    private void OnEnable()
    {
        ControllerManager.enemySpawner = this;
    }

    private void OnDisable()
    {
        ControllerManager.enemySpawner = null;
    }

    private void Awake()
    {
        
    }

    private void Start()
    {
        ActivateNextWave();
    }

    private void Update()
    {
        foreach (var wave in waves)
        {
            if (wave.active && !wave.done && Time.time > wave.GetNextSpawnTime &&
                GetRemainingAlive() < _maxActiveEnemy &&
                (wave.GetRemainingToSpawn > 0 || wave.waveStruct.infinite))
            {
                wave.SetNextSpawnTime(Time.time + wave.waveStruct.timeBetweenSpawn);

                var powerEnemyChanceSO = wave.waveStruct.powerEnemyChance + (wave.waveStruct.powerEnemyChance / 100 * _bonusPowerEnemySpawnChance);

                var zombieType = GetEnemy(wave);
                var powerEnemyChance = Helper.IsCritical(zombieType == ZombieType.FAST
                                                         || zombieType == ZombieType.FAT
                    ? powerEnemyChanceSO * 2
                    : powerEnemyChanceSO);

                SpawnEnemy(GetZombieCountToSpawn(zombieType, powerEnemyChance), zombieType, powerEnemyChance, wave);
            }
            
            if (CheckWaveDone(wave))
            {
                wave.done = true;
                wave.active = false;

                if (wave.finalWave && enemyList.Count <= 0)
                {
                    GameUI.instance.OpenPanel(PanelType.RESULT);
                }
            }
        }

        if (CheckNextWave())
            ActivateNextWave();
    }

    private bool CheckNextWave()
    {
        bool FindPowerEnemyInWave()
        {
            int count = 0;
            
            foreach (var enemy in _currentWave.enemies)
            {
                if (enemy.IsPower) count++;
            }
            
            return count > 2;
        }
        
        var percentAlreadySpawned = _currentWave.GetAlreadySpawned * 100 / _currentWave.waveStruct.enemyCount;
        var percentRemainingAlive = _currentWave.GetRemainingAlive * 100 / _currentWave.waveStruct.enemyCount;

        return percentAlreadySpawned >= 80 && percentRemainingAlive <= 20 && !waves[_currentWaveIndex + 1].active && !FindPowerEnemyInWave();
    }

    private bool CheckWaveDone(Wave wave)
    {
        if (wave.GetAlreadySpawned >= wave.waveStruct.enemyCount && !wave.waveStruct.infinite)
            return true;

        return false;
    }

    private void ActivateNextWave()
    {
        if (waves.Count > 0 && waves[_currentWaveIndex + 1] != null)
        {
            _currentWaveIndex++;
            _currentWave = waves[_currentWaveIndex];
            
            EventBus.SpawnerEvents.OnNewWave?.Invoke(_currentWaveIndex, _currentWave.waveStruct.enemyCount);

            _currentWave.active = true;
            _currentWave.SetRemainingToSpawn(_currentWave.waveStruct.enemyCount);
        }
    }
    
    private int GetRemainingAlive()
    {
        return waves.Sum(wave => wave.GetRemainingAlive);
    }
    
    private int GetZombieCountToSpawn(ZombieType zombieType, bool isCritical)
    {
        int count = 1;

        if (zombieType == ZombieType.FAST)
        {
            count++;

            if (isCritical)
            {
                count += 2;
            }
        }

        return count;
    }

    private ZombieType GetEnemy(Wave wave)
    {
        System.Random random = new System.Random();
        var zombieType = wave.waveStruct.enemyList[random.Next(enemyList.Count)];
            
        switch (zombieType)
        {
            case ZombieType.FAT:
                if (wave.GetFatCount < wave.waveStruct.fatMaxCount)
                {
                    return zombieType;
                }
                break;
            
            case ZombieType.FAST:
                if (wave.GetFastCount < wave.waveStruct.fastMaxCount)
                {
                    return zombieType;
                }
                break;
            
            case ZombieType.STANDARD:
                return zombieType;
            
            default:
                return ZombieType.STANDARD;
        }

        return ZombieType.STANDARD;
    }

    private void SpawnEnemy(int count, ZombieType zombieType, bool isPowerZombie, Wave wave = null)
    {
        Vector3 GetRandomSpawnerPoint()
        {
            if (spawnPoints.Count == 0)
                throw new NotImplementedException("Нет точек спавна для врагов");

            int count = 0;
            var index = -1;

            while (index == -1 && count < 1000)
            {
                System.Random random = new System.Random();
                var randomIndex = random.Next(spawnPoints.Count);
                var randomPoint = spawnPoints[randomIndex];

                if (randomPoint.available)
                    index = randomIndex;

                count++;
            }

            if (index == -1)
                index = 0;

            return spawnPoints[index].transform.position + new Vector3(
                UnityEngine.Random.Range(minSpawnDistance, maxSpawnDistance), 0,
                UnityEngine.Random.Range(minSpawnDistance, maxSpawnDistance));
        }
        
        IEnumerator RemoveEffect(float time, List<ParticleSystem> particleSystems)
        {
            yield return new WaitForSeconds(time * 5);
                
            foreach (var item in particleSystems)
            {
                DeathEffectPool.Instance.ReturnToPool(item);
            }
                
            particleSystems.Clear();
        }

        void ShowEffect(Enemy spawnedEnemy, ProjectileHitInfo projectileHitInfo)
        {
            if (spawnedEnemy.ZombieType == ZombieType.STANDARD || spawnedEnemy.ZombieType == ZombieType.FAST)
            {
                List<ParticleSystem> particleSystems = new();
                
                var particle = DeathEffectPool.Instance.Get();
                particle.transform.position = projectileHitInfo.hitPoint;
                particle.transform.rotation = Quaternion.FromToRotation(Vector3.forward, projectileHitInfo.hitDirection);
                particle.gameObject.SetActive(true);
                
                particleSystems.Add(particle);
                
                StartCoroutine(RemoveEffect(particle.main.duration, particleSystems));
            }

            if (spawnedEnemy.ZombieType == ZombieType.FAT)
            {
                void ShowFatEffect(Vector3 position)
                {
                    spawnedEnemy.OnDeath -= ShowFatEffect;
                    
                    List<ParticleSystem> particleSystems = new();
                    List<Vector3> vectors = new List<Vector3>
                    {
                        Vector3.forward,
                        Vector3.back,
                        Vector3.right,
                        Vector3.left,
                        Vector3.forward - Vector3.left,
                        Vector3.forward - Vector3.right,
                        Vector3.back - Vector3.left,
                        Vector3.back - Vector3.right
                    };

                    foreach (var item in vectors)
                    {
                        var particle = DeathEffectPool.Instance.Get();
                        particle.transform.position = position;
                        particle.transform.rotation = Quaternion.FromToRotation(item, projectileHitInfo.hitDirection);
                        particle.gameObject.SetActive(true);
                        particleSystems.Add(particle);
                    }
                
                    StartCoroutine(RemoveEffect(particleSystems[0].main.duration, particleSystems));
                }

                spawnedEnemy.OnDeath += ShowFatEffect;
            }
        }

        void OnEnemyDeath(GameObject owner, ProjectileHitInfo projectileHitInfo)
        {
            owner.GetComponent<HealthSystem>().OnDeath -= OnEnemyDeath;
            
            var spawnedEnemy = owner.GetComponent<Enemy>();

            if (wave != null)
            {
                wave.SetRemainingAlive(wave.GetRemainingAlive - 1);
                wave.RemoveEnemyFromList(spawnedEnemy);
            }

            enemyList.Remove(spawnedEnemy);
            EventBus.EnemyEvents.OnEnemyCountChange?.Invoke(enemyList.Count);
            
            ShowEffect(spawnedEnemy, projectileHitInfo);

            spawnedEnemy.GetEnemyAttackController.enabled = false;

            if (ControllerManager.experienceSystem != null)
                ControllerManager.experienceSystem.AddXp(spawnedEnemy.XpOnDeath);

            switch (spawnedEnemy.ZombieType)
            {
                case ZombieType.FAT:
                    if (Helper.IsCritical(isPowerZombie ? powerFatBusterSpawnChance : fatBusterSpawnChance))
                    {
                        SpawnBuster(spawnedEnemy);
                    }

                    break;
                case ZombieType.FAST:
                    if (Helper.IsCritical(isPowerZombie ? powerFastBusterSpawnChance : fastBusterSpawnChance))
                    {
                        SpawnBuster(spawnedEnemy);
                    }

                    break;
                case ZombieType.STANDARD:
                    if (Helper.IsCritical(isPowerZombie ? powerStandardBusterSpawnChance : standardBusterSpawnChance))
                    {
                        SpawnBuster(spawnedEnemy);
                    }

                    break;
            }
        }

        void SpawnBuster(Enemy spawnedEnemy)
        {
            if (ControllerManager.busterController == null)
                throw new NotImplementedException("ControllerManager.busterController is null");

            if(ControllerManager.busterController != null)
                ControllerManager.busterController.SpawnBuster(spawnedEnemy.transform.position);
        }

        for (int i = 0; i < count; i++)
        {
            var enemy = EnemyPool.Instance.Get(zombieType, isPowerZombie);

            enemy.Setup(GetRandomSpawnerPoint(), Quaternion.identity, wave);
            
            enemy.gameObject.GetComponent<HealthSystem>().OnDeath += OnEnemyDeath;

            switch (zombieType)
            {
                case ZombieType.FAT:
                    wave?.SetFatCount(wave.GetFatCount + 1);
                    break;
                case ZombieType.FAST:
                    wave?.SetFastCount(wave.GetFastCount + (isPowerZombie ? 4 : 2));
                    break;
            }

            if (wave != null)
            {
                wave.SetRemainingToSpawn(wave.GetRemainingToSpawn - 1);
                wave.SetRemainingAlive(wave.GetRemainingAlive + 1);
                wave.SetAlreadySpawned(wave.GetAlreadySpawned + 1);
                wave.AddEnemyToList(enemy);
                enemyList.Add(enemy);
            }
            
            EventBus.EnemyEvents.OnEnemyCountChange?.Invoke(enemyList.Count);
        }
    }

    public void AddPowerEnemySpawnChance(float value)
    {
        _bonusPowerEnemySpawnChance += value;
    }

    public void AddBonusBusterChance(float percent)
    {
        powerStandardBusterSpawnChance += powerStandardBusterSpawnChance / 100 * percent;
        fastBusterSpawnChance += fastBusterSpawnChance / 100 * percent;
        powerFastBusterSpawnChance += powerFastBusterSpawnChance / 100 * percent;
        fatBusterSpawnChance += fatBusterSpawnChance / 100 * percent;
        powerFatBusterSpawnChance += powerFatBusterSpawnChance / 100 * percent;
    }

    public List<Enemy> GetEnemyList()
    {
        return enemyList;
    }

    #region Test

    [ContextMenu("Spawn 1")]
    public void Spawn1()
    {
        SpawnEnemy(1, ZombieType.STANDARD, false);
    }

    [ContextMenu("Spawn 5")]
    public void Spawn5()
    {
        SpawnEnemy(5, ZombieType.STANDARD, false);
    }
    
    [ContextMenu("Spawn 50")]
    public void Spawn50()
    {
        SpawnEnemy(50, ZombieType.STANDARD, false);
    }
    
    [ContextMenu("Spawn 500")]
    public void Spawn500()
    {
        SpawnEnemy(500, ZombieType.STANDARD, false);
    }

    #endregion
}