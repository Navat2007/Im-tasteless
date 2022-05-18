using System;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public event Action<int, int> OnNewWave;
    
    [SerializeField] private List<Wave> waves = new ();
    [SerializeField] private List<SpawnPoint> spawnPoints = new ();
    [SerializeField] private Enemy standardEnemyPrefab;
    
    [Space(10)]
    [Header("Spawn distance")]
    [SerializeField] private float minSpawnDistance = -5;
    [SerializeField] private float maxSpawnDistance = 5;
    
    [Header("Pools")]
    [SerializeField] private Transform enemyPool;
    [SerializeField] private Transform busterPool;
    
    [Header("Buster spawn chance")]
    [SerializeField] private float standardBusterSpawnChance = 5.0f;
    [SerializeField] private float powerStandardBusterSpawnChance = 100.0f;
    [SerializeField] private float fastBusterSpawnChance = 10.0f;
    [SerializeField] private float powerFastBusterSpawnChance = 50.0f;
    [SerializeField] private float fatBusterSpawnChance = 15.0f;
    [SerializeField] private float powerFatBusterSpawnChance = 100.0f;

    private ExperienceSystem _playerExperienceSystem;
    private BusterController _busterController;
    
    private void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        _playerExperienceSystem = player.GetComponent<ExperienceSystem>();
        _busterController = player.GetComponent<BusterController>();
        
        if(waves.Count > 0)
            ActivateNextWave(waves[0], 0);
    }

    private void Update()
    {
        for (int i = 0; i < waves.Count; i++)
        {
            Wave wave = waves[i];
            
            if (wave.active && !wave.done && Time.time > wave.GetNextSpawnTime && (wave.GetRemainingToSpawn > 0 || wave.waveSO.infinite))
            {
                wave.SetNextSpawnTime(Time.time + wave.waveSO.timeBetweenSpawn);

                var powerEnemyChance = Helper.GetCriticalChance(wave.waveSO.powerEnemyChance);
                var enemy = GetEnemy(wave.waveSO.enemyList, wave, powerEnemyChance);

                SpawnEnemy(enemy, powerEnemyChance, wave);

                if (enemy.ZombieType == ZombieType.FAST)
                {
                    SpawnEnemy(enemy, powerEnemyChance, wave);
                    
                    if (powerEnemyChance)
                    {
                        SpawnEnemy(enemy, powerEnemyChance, wave);
                        SpawnEnemy(enemy, powerEnemyChance, wave);
                    }
                }
            }

            if(i < waves.Count - 1 && !waves[i + 1].done && CheckNextWave(waves[i], waves[i + 1]))
                ActivateNextWave(waves[i + 1], i + 1);

            if (CheckWaveDone(wave))
            {
                wave.done = true;
                wave.active = false;
            }
        }
    }

    private bool CheckNextWave(Wave currentWave, Wave nextWave)
    {
        var percentAlreadySpawned = currentWave.GetAlreadySpawned * 100 / currentWave.waveSO.enemyCount;
        var percentRemainingAlive = currentWave.GetRemainingAlive * 100 / currentWave.waveSO.enemyCount;

        return percentAlreadySpawned >= 80 && percentRemainingAlive <= 20 && !nextWave.active;
    }

    private bool CheckWaveDone(Wave wave)
    {
        if (wave.GetAlreadySpawned >= wave.waveSO.enemyCount && !wave.waveSO.infinite)
            return true;

        return false;
    }

    private void ActivateNextWave(Wave wave, int index)
    {
        //print($"Activate wave {index} with enemy count: {wave.waveSO.enemyCount}");
        
        OnNewWave?.Invoke(index, wave.waveSO.enemyCount);
        
        wave.active = true;
        wave.SetRemainingToSpawn(wave.waveSO.enemyCount);
    }

    private Enemy GetEnemy(List<Enemy> prefabList, Wave wave, bool isPower = false)
    {
        int count = 0;
        var found = false;
        Enemy randomEnemy = standardEnemyPrefab;

        while (!found && count < 1000)
        {
            randomEnemy = prefabList[UnityEngine.Random.Range(0, prefabList.Count)];
            
            switch (randomEnemy.ZombieType)
            {
                case ZombieType.FAT:
                    if (wave.GetStrongCount < wave.waveSO.fatMaxCount)
                    {
                        wave.SetStrongCount(wave.GetStrongCount + 1);
                        found = true;
                    }
                    break;
                case ZombieType.FAST:
                    if (wave.GetFastCount < wave.waveSO.fastMaxCount)
                    {
                        wave.SetFastCount(wave.GetFastCount + (isPower ? 4 : 2));
                        found = true;
                    }
                    break;
                case ZombieType.STANDARD:
                    found = true;
                    break;
            }

            count++;
        }
        
        //print($"Враг заспавнен с {count} попытки");
        
        return randomEnemy;
    }

    private Enemy SpawnEnemy(Enemy enemy, bool isPowerZombie, Wave wave)
    {
        Vector3 GetRandomSpawnerPoint()
        {
            if (spawnPoints.Count == 0)
                throw new NotImplementedException("Нет точек спавна для врагов");
            
            int count = 0;
            var index = -1;

            while (index == -1 && count < 1000)
            {
                var randomIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
                var randomPoint = spawnPoints[randomIndex];

                if (randomPoint.available)
                    index = randomIndex;

                count++;
            }

            if (index == -1)
                index = 0;
            
            //print($"Точка спавна найдена с {count} попытки");

            return spawnPoints[index].transform.position + new Vector3(UnityEngine.Random.Range(minSpawnDistance, maxSpawnDistance), 0, UnityEngine.Random.Range(minSpawnDistance, maxSpawnDistance));
        }
        
        void SetupEnemy(Enemy spawnedEnemy)
        {
            spawnedEnemy.IsPower = isPowerZombie;
            spawnedEnemy.Setup();
        }
        
        void OnEnemyDeath(Enemy spawnedEnemy)
        {
            wave.SetRemainingAlive(wave.GetRemainingAlive - 1);
        
            if(_playerExperienceSystem != null)
                _playerExperienceSystem.AddXp(spawnedEnemy.XpOnDeath);

            switch (spawnedEnemy.ZombieType)
            {
                case ZombieType.FAT:
                    if (Helper.GetCriticalChance(isPowerZombie? powerFatBusterSpawnChance : fatBusterSpawnChance))
                    {
                        SpawnBuster(spawnedEnemy);
                    }
                    break;
                case ZombieType.FAST:
                    if (Helper.GetCriticalChance(isPowerZombie? powerFastBusterSpawnChance : fastBusterSpawnChance))
                    {
                        SpawnBuster(spawnedEnemy);
                    }
                    break;
                case ZombieType.STANDARD:
                    if (Helper.GetCriticalChance(isPowerZombie? powerStandardBusterSpawnChance : standardBusterSpawnChance))
                    {
                        SpawnBuster(spawnedEnemy);
                    }
                    break;
            }
        }

        void SpawnBuster(Enemy spawnedEnemy)
        {
            Array values = Enum.GetValues(typeof(BusterType));
            System.Random random = new System.Random();
            BusterType busterType = (BusterType)values.GetValue(random.Next(values.Length));
            
            Instantiate(
                _busterController.GetBusterPrefab(busterType), 
                spawnedEnemy.transform.position, 
                Quaternion.identity, 
                busterPool
            );
        }
        
        Enemy spawnedEnemy = Instantiate(
            enemy, 
            GetRandomSpawnerPoint(), 
            Quaternion.identity, 
            enemyPool
            );

        wave.SetRemainingToSpawn(wave.GetRemainingToSpawn - 1);
        wave.SetRemainingAlive(wave.GetRemainingAlive + 1);
        wave.SetAlreadySpawned(wave.GetAlreadySpawned + 1);
        
        SetupEnemy(spawnedEnemy);
        
        if (spawnedEnemy.gameObject.TryGetComponent(out HealthSystem healthSystem))
        {
            healthSystem.OnDeath += (hitInfo) => OnEnemyDeath(spawnedEnemy);
        }

        return spawnedEnemy;
    }
}
