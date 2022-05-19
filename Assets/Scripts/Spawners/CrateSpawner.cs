using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CrateSpawner : MonoBehaviour
{
    [SerializeField] private List<SpawnPoint> spawnPoints = new ();
    [SerializeField] private Crate cratePrefab;
    [SerializeField] private Transform cratePool;
    [Space(10)] 
    [SerializeField] private float minTimeBetweenSpawn = 5; 
    [SerializeField] private float maxTimeBetweenSpawn = 15;

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
        Vector3 GetRandomSpawnerPoint()
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

                if (randomPoint.available)
                    index = randomIndex;

                count++;
            }

            if (index == -1)
                index = 0;
            
            //print($"Точка спавна найдена с {count} попытки");

            Vector3 position = new Vector3(spawnPoints[index].transform.position.x, spawnYPosition, spawnPoints[index].transform.position.z);

            return position;
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
            Crate spawnedCrate = Instantiate(
                cratePrefab, 
                GetRandomSpawnerPoint(), 
                Quaternion.identity, 
                cratePool
            );
            
            if (spawnedCrate.gameObject.TryGetComponent(out HealthSystem healthSystem))
            {
                healthSystem.OnDeath += OnCrateDeath;
            }
        }
        
        _isStartSpawn = false;
    }
}
