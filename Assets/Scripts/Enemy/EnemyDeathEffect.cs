using UnityEngine;

public abstract class EnemyDeathEffect : MonoBehaviour
{
    protected Enemy enemy;
    protected Renderer meshRenderer;
    protected HealthSystem healthSystem;
    protected Material material;
    protected Color materialColor;
    
    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        healthSystem = GetComponent<HealthSystem>();
    }

    protected abstract void OnDeath(GameObject owner, ProjectileHitInfo projectileHitInfo);

    public EnemyDeathEffect SetRenderer(Renderer newRenderer)
    {
        meshRenderer = newRenderer;
        material = meshRenderer.material;
        materialColor = material.color;
        
        return this;
    }
}
