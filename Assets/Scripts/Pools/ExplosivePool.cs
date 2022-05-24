using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosivePool : MonoBehaviour
{
    public static ExplosivePool Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private ParticleSystem explosivePrefab;
        
    [Header("Pools")]
    [SerializeField] private Transform explosivePool;

    private Queue<ParticleSystem> _explosives = new();
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
        AddExplosive(10);
    }
        
    private void AddExplosive(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var explosive = Instantiate(explosivePrefab, explosivePool);
            explosive.gameObject.SetActive(false);
            _explosives.Enqueue(explosive);
        }
    }

    public ParticleSystem Get()
    {
        if(_explosives.Count == 0) AddExplosive(1);
        return _explosives.Dequeue();
    }

    public void ReturnToPool(ParticleSystem explosive)
    {
        explosive.gameObject.SetActive(false);
        _explosives.Enqueue(explosive);
    }
}
