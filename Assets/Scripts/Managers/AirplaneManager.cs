using System;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneManager : MonoBehaviour
{
    [SerializeField] private TransportPlane airplane;
    [Space(10)] 
    [SerializeField] private int crateToSpawnCount = 3;
    [SerializeField] private int crateToSpawnTime = 180;

    private float _nextTimeSpawn;

    private void Start()
    {
        airplane.OnSpawn += AirplaneOnSpawnGoods;
    }

    private void Update()
    {
        if (Time.time > _nextTimeSpawn)
        {
            _nextTimeSpawn = Time.time + crateToSpawnTime;
            
            LaunchPlane();
        }
    }

    [ContextMenu("Launch")]
    public void LaunchPlane()
    {
        airplane.Reset();
        airplane.gameObject.SetActive(true);
    }
    
    [ContextMenu("Spawn crate")]
    private void AirplaneOnSpawnGoods()
    {
        StartCoroutine(ControllerManager.crateSpawner.SpawnCrate(crateToSpawnCount, 40, 0, true));
    }
}
