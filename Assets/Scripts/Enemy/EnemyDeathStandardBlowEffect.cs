using UnityEngine;

public class EnemyDeathStandardBlowEffect : EnemyDeathEffect
{
    protected override void OnDeath(ProjectileHitInfo projectileHitInfo)
    {
        base.OnDeath(projectileHitInfo);
        
        var deathEffectRenderer = deathEffectParticlePrefab.GetComponent<ParticleSystemRenderer>();
        deathEffectRenderer.material = meshRenderer.material;
            
        var deathEffectGameObject = Instantiate(
            deathEffectParticlePrefab.gameObject,
            projectileHitInfo.hitPoint,
            Quaternion.FromToRotation(Vector3.forward, projectileHitInfo.hitDirection));
            
        Destroy(deathEffectGameObject, deathEffectParticlePrefab.main.startLifetime.constantMax);
        Destroy(gameObject);
    }
}
