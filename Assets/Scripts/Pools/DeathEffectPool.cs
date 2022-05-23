using System.Collections.Generic;
using UnityEngine;

public class DeathEffectPool : MonoBehaviour
{
    public static DeathEffectPool Instance { get; private set; }

    [Header("Prefabs")] 
    [SerializeField] private ParticleSystem deathEffectPrefab;

    [Header("Pools")] 
    [SerializeField] private Transform deathEffectPool;

    private Queue<ParticleSystem> _deathEffects = new();

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
        AddDeathEffect(30);
    }

    private void AddDeathEffect(int count)
    {
        var deathEffect = Instantiate(deathEffectPrefab, deathEffectPool);
        deathEffect.gameObject.SetActive(false);
        _deathEffects.Enqueue(deathEffect);
    }

    public ParticleSystem Get()
    {
        if (_deathEffects.Count == 0) AddDeathEffect(1);
        return _deathEffects.Dequeue();
    }

    public void ReturnToPool(ParticleSystem deathEffect)
    {
        deathEffect.gameObject.SetActive(false);
        _deathEffects.Enqueue(deathEffect);
    }
}