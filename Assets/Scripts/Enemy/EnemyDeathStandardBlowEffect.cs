using UnityEngine;

public class EnemyDeathStandardBlowEffect : EnemyDeathEffect
{
    private ParticleSystem _particleSystem;
    private float _timer;
    private bool _spawned;
    
    private void Update()
    {
        if (_spawned && Time.time > _timer)
        {
            DeathEffectPool.Instance.ReturnToPool(_particleSystem);
        }
    }

    protected override void OnDeath(ProjectileHitInfo projectileHitInfo)
    {
        base.OnDeath(projectileHitInfo);

        _particleSystem = DeathEffectPool.Instance.Get();
        _particleSystem.transform.position = projectileHitInfo.hitPoint;
        _particleSystem.transform.rotation = Quaternion.FromToRotation(Vector3.forward, projectileHitInfo.hitDirection);
        _particleSystem.gameObject.SetActive(true);

        _timer = Time.time + _particleSystem.main.startLifetime.constantMax;
        _spawned = true;
        enemy.GetEnemyController.OnDeath();
        
        StartCoroutine(Fade(materialColor, Color.grey, 10));
    }
}