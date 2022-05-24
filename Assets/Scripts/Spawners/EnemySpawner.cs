using System;
using System.Collections;
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
        int GetZombieCountToSpawn(ZombieType zombieType, bool isCritical)
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
        
        for (int i = 0; i < waves.Count; i++)
        {
            Wave wave = waves[i];

            if (wave.active && !wave.done && Time.time > wave.GetNextSpawnTime &&
                (wave.GetRemainingToSpawn > 0 || wave.waveStruct.infinite))
            {
                wave.SetNextSpawnTime(Time.time + wave.waveStruct.timeBetweenSpawn);

                var powerEnemyChanceSO = wave.waveStruct.powerEnemyChance + (wave.waveStruct.powerEnemyChance / 100 * _bonusPowerEnemySpawnChance);

                var zombieType = GetEnemy(wave.waveStruct.enemyList, wave);
                var powerEnemyChance = Helper.IsCritical(zombieType == ZombieType.FAST
                                                         || zombieType == ZombieType.FAT
                    ? powerEnemyChanceSO * 2
                    : powerEnemyChanceSO);

                SpawnEnemy(GetZombieCountToSpawn(zombieType, powerEnemyChance), zombieType, powerEnemyChance, wave);
            }

            if (i < waves.Count - 1 && !waves[i + 1].done && CheckNextWave(waves[i], waves[i + 1]))
                ActivateNextWave(waves[i + 1], i + 1);

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

    private ZombieType GetEnemy(List<ZombieType> enemyList, Wave wave)
    {
        int count = 0;

        while (count < 1000)
        {
            System.Random random = new System.Random();
            var zombieType = enemyList[random.Next(enemyList.Count)];
            
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
            }

            count++;
        }

        return ZombieType.STANDARD;
    }

    private void SpawnEnemy(int count, ZombieType zombieType, bool isPowerZombie, Wave wave)
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

        void ShowEffect(Enemy spawnedEnemy, ProjectileHitInfo projectileHitInfo)
        {
            IEnumerator Remove(float time, List<ParticleSystem> particleSystems)
            {
                yield return new WaitForSeconds(time * 5);
                
                print($"Remove after {time}: {particleSystems.Count}");
                
                foreach (var item in particleSystems)
                {
                    DeathEffectPool.Instance.ReturnToPool(item);
                }
                
                particleSystems.Clear();
            }
            
            if (spawnedEnemy.ZombieType == ZombieType.STANDARD || spawnedEnemy.ZombieType == ZombieType.FAST)
            {
                List<ParticleSystem> particleSystems = new();
                
                var particle = DeathEffectPool.Instance.Get();
                particle.transform.position = projectileHitInfo.hitPoint;
                particle.transform.rotation = Quaternion.FromToRotation(Vector3.forward, projectileHitInfo.hitDirection);
                particle.gameObject.SetActive(true);
                
                particleSystems.Add(particle);
                
                StartCoroutine(Remove(particle.main.duration, particleSystems));
            }

            if (spawnedEnemy.ZombieType == ZombieType.FAT)
            {
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
                    particle.transform.position = transform.position;
                    particle.transform.rotation = Quaternion.FromToRotation(item, projectileHitInfo.hitDirection);
                    particle.gameObject.SetActive(true);
                    particleSystems.Add(particle);
                }
                
                print(particleSystems.Count);
                
                StartCoroutine(Remove(particleSystems[0].main.duration, particleSystems));
            }
        }

        void OnEnemyDeath(Enemy spawnedEnemy, ProjectileHitInfo projectileHitInfo)
        {
            wave.SetRemainingAlive(wave.GetRemainingAlive - 1);
            wave.RemoveEnemyFromList(spawnedEnemy);
            enemyList.Remove(spawnedEnemy);
            OnEnemyCountChange?.Invoke(enemyList.Count);
            
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

            enemy
                .Setup(GetRandomSpawnerPoint(), Quaternion.identity, wave)
                .gameObject.SetActive(true);

            switch (zombieType)
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
            wave.AddEnemyToList(enemy);
            enemyList.Add(enemy);
            OnEnemyCountChange?.Invoke(enemyList.Count);

            if (enemy.gameObject.TryGetComponent(out HealthSystem healthSystem))
            {
                healthSystem.OnDeath += (hitInfo) => OnEnemyDeath(enemy, hitInfo);
            }
        }
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