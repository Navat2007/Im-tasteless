using UnityEngine;

public class EnemyDeathEffect : MonoBehaviour
{
    [Header("Настройки при смерти")] 
    [SerializeField] private ParticleSystem deathEffect;
    
    private Enemy _enemy;
    private MeshRenderer _meshRenderer;
    private HealthSystem _healthSystem;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _healthSystem = GetComponent<HealthSystem>();
    }

    private void OnEnable()
    {
        _healthSystem.OnDeath += OnDeath;
    }

    private void OnDisable()
    {
        _healthSystem.OnDeath -= OnDeath;
    }
    
    private void OnDeath(ProjectileHitInfo projectileHitInfo)
    {
        var deathEffectRenderer = deathEffect.GetComponent<ParticleSystemRenderer>();
        deathEffectRenderer.material = _meshRenderer.material;
            
        var deathEffectGameObject = Instantiate(
            deathEffect.gameObject,
            projectileHitInfo.hitPoint,
            Quaternion.FromToRotation(Vector3.forward, projectileHitInfo.hitDirection));
            
        Destroy(deathEffectGameObject, deathEffect.main.startLifetime.constantMax);
    }
}
