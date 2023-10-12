using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pools;
using UnityEngine;
using Random = UnityEngine.Random;

public class CrateSpawner : MonoBehaviour
{
    public bool IsNextPowerCrate { get; private set; }
    public bool IsDoublePowerCrate { get; private set; }
    
    [SerializeField] private List<SpawnPoint> spawnPoints = new ();
    [SerializeField] private Crate cratePrefab;
    [SerializeField] private Transform cratePool;
    [Space(10)] 
    [SerializeField] private float minTimeBetweenSpawn = 5; 
    [SerializeField] private float maxTimeBetweenSpawn = 15;

    private List<Crate> _crateList = new();
    private List<SpawnPoint> _currentSpawnPoints = new();

    private float _nextScanTime;
    private bool _isStartSpawn;
    private bool _isUIOpen;

    private void OnEnable()
    {
        GameUI.instance.OnUIOpen += OnUIOpen;
        GameUI.instance.OnUIClose += OnUIClose;
        ControllerManager.crateSpawner = this;
    }

    private void OnDisable()
    {
        GameUI.instance.OnUIOpen -= OnUIOpen;
        GameUI.instance.OnUIClose -= OnUIClose;
        ControllerManager.crateSpawner = null;
    }
    
    private void OnUIOpen()
    {
        _isUIOpen = true;
    }
    
    private void OnUIClose()
    {
        _isUIOpen = false;
    }

    private void Update()
    {
        if (Time.time > _nextScanTime && ControllerManager.player != null)
        {
            if (_crateList.Count == 0 || _isUIOpen) return;
            
            _nextScanTime = Time.time + 1;

            float minDistance = Mathf.Infinity;
            int index = -1;

            for (int i = 0; i < _crateList.Count; i++)
            {
                var cratePointer = _crateList[i].gameObject.GetComponentInChildren<CratePointer>();
                
                if (cratePointer != null)
                {
                    if(cratePointer.IsFade && cratePointer.IsShow)
                        cratePointer.SetInvisible();
                    
                    float distance = Vector3.Distance(_crateList[i].transform.position, ControllerManager.player.transform.position);
                    
                    if (distance < minDistance)
                    {
                        index = i;
                        minDistance = distance;
                    }
                }
            }
            
            if(index >= 0)
            {
                var pointer = _crateList[index].gameObject.GetComponentInChildren<CratePointer>();
            
                if (pointer != null && index >= 0)
                {
                    pointer.SetVisible();
                }
            }
        }
    }
    
    private List<SpawnPoint> GetAvailableSpawnPoints()
    {
        return spawnPoints.Where(spawnPoint => spawnPoint.available && !_currentSpawnPoints.Contains(spawnPoint)).ToList();
    }

    public IEnumerator SpawnCrate(int count, float spawnYPosition, float spawnDelaySeconds, bool airSpawn)
    {
        SpawnPoint GetRandomSpawnerPoint()
        {
            if (spawnPoints.Count == 0)
                throw new NotImplementedException("Нет точек спавна для ящиков");
            
            var availableSpawnPoints = GetAvailableSpawnPoints();
            var index = UnityEngine.Random.Range(0, availableSpawnPoints.Count);
            var spawnPoint = availableSpawnPoints[index];
            
            return spawnPoint;
        }
        
        void OnCrateDeath(GameObject owner, ProjectileHitInfo projectileHitInfo)
        {
            owner.GetComponent<HealthSystem>().OnDeath -= OnCrateDeath;
            _crateList.Remove(owner.GetComponent<Crate>());
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

            var crate = CratePool.Instance.Get();
            
            crate
                .Setup(position, Quaternion.identity)
                .SetSpawnPoint(point)
                .gameObject.SetActive(true);
            
            crate.GetComponent<HealthSystem>().OnDeath += OnCrateDeath;

            _crateList.Add(crate);
            _currentSpawnPoints.Add(point);
        }
        
        _isStartSpawn = false;
    }

    public void SetPowerCrate(bool powerCrate, bool doublePower)
    {
        IsNextPowerCrate = powerCrate;
        IsDoublePowerCrate = doublePower;
    }

    public void RemoveSpawnPoint(SpawnPoint point)
    {
        var index = _currentSpawnPoints.IndexOf(point);
        _currentSpawnPoints.RemoveAt(index);
    }
}