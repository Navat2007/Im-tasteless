using System;
using UnityEngine;

public abstract class EnemyDeathEffect : MonoBehaviour
{
    [Header("Настройки при смерти")] 
    [SerializeField] protected ParticleSystem deathEffectParticlePrefab;
    
    protected Enemy enemy;
    protected MeshRenderer meshRenderer;
    protected HealthSystem healthSystem;
    protected Material material;
    protected Color materialColor;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        meshRenderer = GetComponent<MeshRenderer>();
        healthSystem = GetComponent<HealthSystem>();
        material = meshRenderer.material;
        materialColor = material.color;
        
        if (deathEffectParticlePrefab == null)
            throw new NotImplementedException("EnemyDeathEffect: не прикреплён ParticleSystem префаб");
    }

    private void OnEnable()
    {
        healthSystem.OnDeath += OnDeath;
    }

    private void OnDisable()
    {
        healthSystem.OnDeath -= OnDeath;
    }

    protected virtual void OnDeath(ProjectileHitInfo projectileHitInfo)
    {
        healthSystem.enabled = false;
    }
}
