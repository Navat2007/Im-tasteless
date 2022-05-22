using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public event Action<int, int> OnNewWave;
    public event Action<int> OnEnemyCountChange;

    [SerializeField] private List<Wave> waves = new();
    
    [Header("Точки спавна на карте")]
    [SerializeField] private List<SpawnPoint> spawnPoints = new();
    
    [Header("Стандартный враг, если не найдены остальные")]
    [SerializeField] private Enemy standardEnemyPrefab;

    [Space(10)]
    [Header("Дистанция спавна")]
    [SerializeField] private float minSpawnDistance = -5;

    [SerializeField] private float maxSpawnDistance = 5;

    [Header("Хранилище объектов")]
    [SerializeField] private Transform enemyPool;
    [SerializeField] private List<Enemy> enemyList;

    [Header("Шанс спавна бустеров")]
    [SerializeField] private float standardBusterSpawnChance = 5.0f;

    [SerializeField] private float powerStandardBusterSpawnChance = 100.0f;
    [SerializeField] private float fastBusterSpawnChance = 10.0f;
    [SerializeField] private float powerFastBusterSpawnChance = 50.0f;
    [SerializeField] private float fatBusterSpawnChance = 15.0f;
    [SerializeField] private float powerFatBusterSpawnChance = 100.0f;

    private float _bonusPowerEnemySpawnChance;

    private void OnEnable()
    {
        ControllerManager.enemySpawner = this;
    }

    private void OnDisable()
    {
        ControllerManager.enemySpawner = null;
    }

    private void Start()
    {
        if (waves.Count > 0)
            ActivateNextWave(waves[0], 0);
    }

    private void Update()
    {
        for (int i = 0; i < waves.Count; i++)
        {
            Wave wave = waves[i];

            if (wave.active && !wave.done && Time.time > wave.GetNextSpawnTime &&
                (wave.GetRemainingToSpawn > 0 || wave.waveStruct.infinite))
            {
                wave.SetNextSpawnTime(Time.time + wave.waveStruct.timeBetweenSpawn);

                var powerEnemyChanceSO = wave.waveStruct.powerEnemyChance + (wave.waveStruct.powerEnemyChance / 100 * _bonusPowerEnemySpawnChance);

                var enemy = GetEnemy(wave.waveStruct.enemyList, wave);
                var powerEnemyChance = Helper.IsCritical(enemy.ZombieType == ZombieType.FAST
                                                         || enemy.ZombieType == ZombieType.FAT
                    ? powerEnemyChanceSO * 2
                    : powerEnemyChanceSO);

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

            if (i < waves.Count - 1 && !waves[i + 1].done && CheckNextWave(waves[i], waves[i + 1]))
                ActivateNextWave(waves[i + 1], i + 1);

            if (CheckWaveDone(wave))
            {
                wave.done = true;
                wave.active = false;

                if (wave.finalWave && enemyPool.childCount <= 0)
                {
                    GameUI.instance.OpenPanel(PanelType.RESULT);
                }
            }
        }
    }

    private bool CheckNextWave(Wave currentWave, Wave nextWave)
    {
        bool FindPowerEnemyInWave()
        {
            foreach (var enemy in currentWave.enemies)
            {
                if (enemy.IsPower) return true;
            }
            
            return false;
        }
        
        var percentAlreadySpawned = currentWave.GetAlreadySpawned * 100 / currentWave.waveStruct.enemyCount;
        var percentRemainingAlive = currentWave.GetRemainingAlive * 100 / currentWave.waveStruct.enemyCount;

        return percentAlreadySpawned >= 80 && percentRemainingAlive <= 20 && !nextWave.active && !FindPowerEnemyInWave();
    }

    private bool CheckWaveDone(Wave wave)
    {
        if (wave.GetAlreadySpawned >= wave.waveStruct.enemyCount && !wave.waveStruct.infinite)
            return true;

        return false;
    }

    private void ActivateNextWave(Wave wave, int index)
    {
        OnNewWave?.Invoke(index, wave.waveStruct.enemyCount);

        wave.active = true;
        wave.SetRemainingToSpawn(wave.waveStruct.enemyCount);
    }

    private Enemy GetEnemy(List<Enemy> prefabList, Wave wave)
    {
        int count = 0;
        var found = false;
        Enemy randomEnemy = standardEnemyPrefab;

        while (!found && count < 1000)
        {
            System.Random random = new System.Random();
            randomEnemy = prefabList[random.Next(prefabList.Count)];

            switch (randomEnemy.ZombieType)
            {
                case ZombieType.FAT:
                    if (wave.GetFatCount < wave.waveStruct.fatMaxCount)
                    {
                        found = true;
                    }

                    break;
                case ZombieType.FAST:
                    if (wave.GetFastCount < wave.waveStruct.fastMaxCount)
                    {
                        found = true;
                    }

                    break;
                case ZombieType.STANDARD:
                    found = true;
                    break;
            }

            count++;
        }

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

        void SetupEnemy(Enemy spawnedEnemy)
        {
            spawnedEnemy.IsPower = isPowerZombie;
            spawnedEnemy.Setup(wave);
        }

        void OnEnemyDeath(Enemy spawnedEnemy)
        {
            wave.SetRemainingAlive(wave.GetRemainingAlive - 1);
            wave.RemoveEnemyFromList(spawnedEnemy);
            enemyList.Remove(spawnedEnemy);
            OnEnemyCountChange?.Invoke(enemyPool.childCount);

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

        Enemy spawnedEnemy = Instantiate(
            enemy,
            GetRandomSpawnerPoint(),
            Quaternion.identity,
            enemyPool
        );

        switch (spawnedEnemy.ZombieType)
        {
            case ZombieType.FAT:
                wave.SetFatCount(wave.GetFatCount + 1);
                break;
            case ZombieType.FAST:
                wave.SetFastCount(wave.GetFastCount + (isPowerZombie ? 4 : 2));
                break;
        }

        wave.SetRemainingToSpawn(wave.GetRemainingToSpawn - 1);
        wave.SetRemainingAlive(wave.GetRemainingAlive + 1);
        wave.SetAlreadySpawned(wave.GetAlreadySpawned + 1);
        wave.AddEnemyToList(spawnedEnemy);
        enemyList.Add(spawnedEnemy);
        OnEnemyCountChange?.Invoke(enemyPool.childCount);

        SetupEnemy(spawnedEnemy);

        if (spawnedEnemy.gameObject.TryGetComponent(out HealthSystem healthSystem))
        {
            healthSystem.OnDeath += (hitInfo) => OnEnemyDeath(spawnedEnemy);
        }

        return spawnedEnemy;
    }

    public void AddPowerEnemySpawnChance(float value)
    {
        _bonusPowerEnemySpawnChance += value;
    }

    public List<Enemy> GetEnemyList()
    {
        return enemyList;
    }
}