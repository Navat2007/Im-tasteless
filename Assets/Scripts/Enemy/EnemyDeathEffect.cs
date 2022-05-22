using System;
using System.Collections;
using UnityEngine;

public abstract class EnemyDeathEffect : MonoBehaviour
{
    [Header("Настройки при смерти")] 
    [SerializeField] protected ParticleSystem deathEffectParticlePrefab;
    
    protected Enemy enemy;
    protected Renderer meshRenderer;
    protected HealthSystem healthSystem;
    protected Material material;
    protected Color materialColor;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        healthSystem = GetComponent<HealthSystem>();
        
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

    protected IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 1;

        while (percent > 0)
        {
            percent -= Time.deltaTime * speed;
            material.color = Color.Lerp(to, from, percent);
            yield return null;
        }
        
        Destroy(gameObject);
    }

    protected virtual void OnDeath(ProjectileHitInfo projectileHitInfo)
    {
        healthSystem.enabled = false;
    }

    public EnemyDeathEffect SerRenderer(Renderer newRenderer)
    {
        meshRenderer = newRenderer;
        material = meshRenderer.material;
        materialColor = material.color;
        
        return this;
    }
}
