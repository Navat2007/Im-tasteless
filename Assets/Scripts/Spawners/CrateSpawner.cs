using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CrateSpawner : MonoBehaviour
{
    public bool IsNextPowerCrate { get; private set; }
    
    [SerializeField] private List<SpawnPoint> spawnPoints = new ();
    [SerializeField] private Crate cratePrefab;
    [SerializeField] private Transform cratePool;
    [Space(10)] 
    [SerializeField] private float minTimeBetweenSpawn = 5; 
    [SerializeField] private float maxTimeBetweenSpawn = 15;

    private List<SpawnPoint> _currentSpawnPoints = new();
    private bool _isStartSpawn;

    private void OnEnable()
    {
        ControllerManager.crateSpawner = this;
    }

    private void OnDisable()
    {
        ControllerManager.crateSpawner = null;
    }

    public IEnumerator SpawnCrate(int count, float spawnYPosition, float spawnDelaySeconds, bool airSpawn)
    {
        SpawnPoint GetRandomSpawnerPoint()
        {
            if (spawnPoints.Count == 0)
                throw new NotImplementedException("Нет точек спавна для ящиков");
            
            int count = 0;
            var index = -1;

            while (index == -1 && count < 1000)
            {
                System.Random random = new System.Random();
                var randomIndex = random.Next(spawnPoints.Count);
                var randomPoint = spawnPoints[randomIndex];

                if (randomPoint.available && !_currentSpawnPoints.Contains(randomPoint))
                    index = randomIndex;

                count++;
            }

            if (index == -1)
                index = 0;
            
            //print($"Точка спавна найдена с {count} попытки");

            

            return spawnPoints[index];
        }
        
        void OnCrateDeath(ProjectileHitInfo projectileHitInfo)
        {
            StartCoroutine(SpawnCrate(1, 0.4f, Random.Range(minTimeBetweenSpawn, maxTimeBetweenSpawn), false));
        }

        if (_isStartSpawn && !airSpawn)
            yield break;

        _isStartSpawn = true;

        yield return new WaitForSeconds(spawnDelaySeconds);

        for (int i = 0; i < count; i++)
        {
            SpawnPoint point = GetRandomSpawnerPoint();
            Vector3 position = new Vector3(point.transform.position.x, spawnYPosition, point.transform.position.z);
            
            Crate spawnedCrate = Instantiate(
                cratePrefab, 
                position, 
                Quaternion.identity, 
                cratePool
            );
            
            _currentSpawnPoints.Add(point);
            spawnedCrate.SetSpawnPoint(point);
            
            if (spawnedCrate.gameObject.TryGetComponent(out HealthSystem healthSystem))
            {
                healthSystem.OnDeath += OnCrateDeath;
            }
        }
        
        _isStartSpawn = false;
    }

    public void SetPowerCrate(bool value)
    {
        IsNextPowerCrate = value;
    }

    public void RemoveSpawnPoint(SpawnPoint point)
    {
        var index = _currentSpawnPoints.IndexOf(point);
        _currentSpawnPoints.RemoveAt(index);
    }
}