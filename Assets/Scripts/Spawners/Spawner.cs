using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public event Action<int, int> OnNewWave;
    
    [SerializeField] private List<Wave> waves = new ();
    [SerializeField] private List<SpawnPoint> spawnPoints = new ();
    [Space(10)]
    [SerializeField] private float minSpawnDistance = -5;
    [SerializeField] private float maxSpawnDistance = 5;
    [SerializeField] private float busterSpawnChance = 12.5f;
    [SerializeField] private Transform enemyPool;
    [SerializeField] private Transform busterPool;

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
            
            //print($"Wave {i}: active - {wave.active}, is time to spawn - {Time.time > wave.GetNextSpawnTime}, remaining to spawn - {wave.GetRemainingToSpawn}");
            
            if (wave.active && !wave.done && Time.time > wave.GetNextSpawnTime && (wave.GetRemainingToSpawn > 0 || wave.waveSO.infinite))
            {
                wave.SetNextSpawnTime(Time.time + wave.waveSO.timeBetweenSpawn);

                var enemy = GetEnemy(wave.waveSO.enemyList, wave);
                var powerEnemyChance = Helper.GetCriticalChance(wave.waveSO.powerEnemyChance);
                
                SpawnEnemy(enemy, powerEnemyChance, wave);
                
                if(enemy.ZombieType == ZombieType.FAST)
                    SpawnEnemy(enemy, powerEnemyChance, wave);
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
        //print($"Current wave percent: {percent}");

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

    private Enemy GetEnemy(List<Enemy> prefabList, Wave wave)
    {
        int count = 0;
        var found = false;
        var randomEnemy = prefabList[UnityEngine.Random.Range(0, prefabList.Count)];

        while (!found && count < 1000)
        {
            Array values = Enum.GetValues(typeof(ZombieType));
            System.Random random = new System.Random();
            ZombieType zombieType = (ZombieType)values.GetValue(random.Next(values.Length));
            
            switch (zombieType)
            {
                case ZombieType.STRONG:
                    if (wave.GetStrongCount < wave.waveSO.strongMaxCount)
                    {
                        randomEnemy.SetZombieType(zombieType);
                        wave.SetStrongCount(wave.GetStrongCount + 1);
                        found = true;
                    }
                    break;
                case ZombieType.FAST:
                    if (wave.GetFastCount < wave.waveSO.fastMaxCount)
                    {
                        randomEnemy.SetZombieType(zombieType);
                        wave.SetFastCount(wave.GetFastCount + 1);
                        found = true;
                    }
                    break;
                case ZombieType.STANDARD:
                    randomEnemy.SetZombieType(zombieType);
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
            spawnedEnemy.SetSize(Vector3.one);
            
            switch (spawnedEnemy.ZombieType)
            {
                case ZombieType.STRONG:
                    spawnedEnemy.SetHealth(isPowerZombie ? wave.waveSO.health * 30 : wave.waveSO.health * 3);
                    spawnedEnemy.SetDamage(isPowerZombie ? wave.waveSO.damage * 6 : wave.waveSO.damage * 2);
                    spawnedEnemy.SetMoveSpeed(isPowerZombie ? wave.waveSO.moveSpeed * 0.7f : wave.waveSO.moveSpeed * 0.55f);
                    spawnedEnemy.SetXpOnDeath(isPowerZombie ? wave.waveSO.xpOnDeath * 20 : wave.waveSO.xpOnDeath * 2);
                    spawnedEnemy.SetColor(wave.waveSO.colorStrong);
                    spawnedEnemy.SetSize(spawnedEnemy.transform.localScale * 1.3f);
                    break;
                case ZombieType.FAST:
                    spawnedEnemy.SetHealth(isPowerZombie ? wave.waveSO.health * 7.5f : wave.waveSO.health * 0.75f);
                    spawnedEnemy.SetDamage(isPowerZombie ? wave.waveSO.damage * 3 : wave.waveSO.damage);
                    spawnedEnemy.SetMoveSpeed(isPowerZombie ? wave.waveSO.moveSpeed * 1.15f : wave.waveSO.moveSpeed);
                    spawnedEnemy.SetXpOnDeath(isPowerZombie ? wave.waveSO.xpOnDeath * 10 : wave.waveSO.xpOnDeath);
                    spawnedEnemy.SetColor(wave.waveSO.colorFast);
                    spawnedEnemy.SetSize(spawnedEnemy.transform.localScale * 0.9f);
                    break;
                case ZombieType.STANDARD:
                    spawnedEnemy.SetHealth(isPowerZombie ? wave.waveSO.health * 10 : wave.waveSO.health);
                    spawnedEnemy.SetDamage(isPowerZombie ? wave.waveSO.damage * 3 : wave.waveSO.damage);
                    spawnedEnemy.SetMoveSpeed(isPowerZombie ? wave.waveSO.moveSpeed * 0.7f : wave.waveSO.moveSpeed * 0.75f);
                    spawnedEnemy.SetXpOnDeath(isPowerZombie ? wave.waveSO.xpOnDeath * 10 : wave.waveSO.xpOnDeath);
                    spawnedEnemy.SetColor(wave.waveSO.colorStandard);
                    break;
            }

            if (isPowerZombie)
            {
                var powerSkin = spawnedEnemy.AddComponent<EnemyPowerSkin>();
                powerSkin.SetColor(spawnedEnemy.GetColor());
            }
        }
        
        void OnEnemyDeath(Enemy spawnedEnemy)
        {
            wave.SetRemainingAlive(wave.GetRemainingAlive - 1);
        
            if(_playerExperienceSystem != null)
                _playerExperienceSystem.AddXp(spawnedEnemy.XpOnDeath);

            if (Helper.GetCriticalChance(busterSpawnChance))
            {
                SpawnBuster(spawnedEnemy);
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
            healthSystem.OnDeath += () => OnEnemyDeath(spawnedEnemy);
        }

        return spawnedEnemy;
    }
}
