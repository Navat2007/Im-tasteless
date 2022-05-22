using System;
using System.Collections;
using Interface;
using UnityEngine;

public class EnemyDeathFatBlowEffect : EnemyDeathEffect
{
    [SerializeField] private float timer = 2;
    [SerializeField] private float radius = 5;
    [SerializeField] private float force = 600;
    [SerializeField] private float damage = 10;
    [SerializeField] private float upwardsModifier = 0;
    [SerializeField] private ForceMode forceMode;
    [SerializeField] private float stunTime = 1;
    [SerializeField] private AudioClip explodeClip;

    private float _growSpeed = 0.1f;
    private bool _isExploded;

    protected override void OnDeath(ProjectileHitInfo projectileHitInfo)
    {
        if (_isExploded)
            return;

        _isExploded = true;
        base.OnDeath(projectileHitInfo);
        
        if(gameObject.TryGetComponent(out EnemyPowerSkin skin))
        {
            skin.Switch();
        }
        
        if(gameObject.TryGetComponent(out EnemyController controller))
        {
            controller.ResetBonusSpeed();
        }

        if (enemy.IsPower)
        {
            timer += 1;
            radius += 3;
            force *= 2;
            damage *= 3;
        }
        
        IEnumerator Timer()
        {
            Color startColor = material.color;
            float timeToExplode = Time.time + timer;
            
            while (Time.time < timeToExplode)
            {
                transform.localScale += Vector3.one * _growSpeed;
                
                material.color = Color.red * 2;
                yield return new WaitForSeconds(0.1f);
                material.color = startColor * 1;
                yield return new WaitForSeconds(0.1f);
            }

            ShowEffect();
            AddForce();
            ExplodeSound();
            
            enemy.GetEnemyController.OnDeath();
            Destroy(gameObject);
        }

        void ShowEffect()
        {
            //var deathEffectRenderer = deathEffectParticlePrefab.GetComponent<ParticleSystemRenderer>();
            //deathEffectRenderer.material.color = materialColor;

            var deathEffectGameObject = Instantiate(
                deathEffectParticlePrefab.gameObject,
                transform.position,
                Quaternion.FromToRotation(Vector3.forward, projectileHitInfo.hitDirection));

            var deathEffectGameObject2 = Instantiate(
                deathEffectParticlePrefab.gameObject,
                transform.position,
                Quaternion.FromToRotation(Vector3.back, projectileHitInfo.hitDirection));

            var deathEffectGameObject3 = Instantiate(
                deathEffectParticlePrefab.gameObject,
                transform.position,
                Quaternion.FromToRotation(Vector3.right, projectileHitInfo.hitDirection));

            var deathEffectGameObject4 = Instantiate(
                deathEffectParticlePrefab.gameObject,
                transform.position,
                Quaternion.FromToRotation(Vector3.left, projectileHitInfo.hitDirection));

            var deathEffectGameObject5 = Instantiate(
                deathEffectParticlePrefab.gameObject,
                transform.position,
                Quaternion.FromToRotation(Vector3.forward - Vector3.left, projectileHitInfo.hitDirection));

            var deathEffectGameObject6 = Instantiate(
                deathEffectParticlePrefab.gameObject,
                transform.position,
                Quaternion.FromToRotation(Vector3.forward - Vector3.right, projectileHitInfo.hitDirection));

            var deathEffectGameObject7 = Instantiate(
                deathEffectParticlePrefab.gameObject,
                transform.position,
                Quaternion.FromToRotation(Vector3.back - Vector3.left, projectileHitInfo.hitDirection));

            var deathEffectGameObject8 = Instantiate(
                deathEffectParticlePrefab.gameObject,
                transform.position,
                Quaternion.FromToRotation(Vector3.back - Vector3.right, projectileHitInfo.hitDirection));

            Destroy(deathEffectGameObject, deathEffectParticlePrefab.main.startLifetime.constantMax);
            Destroy(deathEffectGameObject2, deathEffectParticlePrefab.main.startLifetime.constantMax);
            Destroy(deathEffectGameObject3, deathEffectParticlePrefab.main.startLifetime.constantMax);
            Destroy(deathEffectGameObject4, deathEffectParticlePrefab.main.startLifetime.constantMax);
            Destroy(deathEffectGameObject5, deathEffectParticlePrefab.main.startLifetime.constantMax);
            Destroy(deathEffectGameObject6, deathEffectParticlePrefab.main.startLifetime.constantMax);
            Destroy(deathEffectGameObject7, deathEffectParticlePrefab.main.startLifetime.constantMax);
            Destroy(deathEffectGameObject8, deathEffectParticlePrefab.main.startLifetime.constantMax);
        }

        void AddForce()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

            foreach (var nearbyObjects in colliders)
            {
                if (nearbyObjects.TryGetComponent(out Rigidbody rigidbody))
                {
                    if (nearbyObjects.TryGetComponent(out EnemyController controller))
                    {
                        controller.StopNavMeshAgent();
                        controller.StartNavMeshAgent(stunTime);
                    }
                    rigidbody.AddExplosionForce(force, transform.position, radius, upwardsModifier, forceMode);
                }

                if (nearbyObjects.GetComponent<IDamageable>() != null &&
                    nearbyObjects.TryGetComponent(out HealthSystem healthSystem))
                {
                    var heading = nearbyObjects.transform.position - transform.position;
                    var distance = heading.magnitude;
                    var direction = heading / distance;

                    healthSystem.TakeDamage(new ProjectileHitInfo
                    {
                        damage = damage,
                        isCritical = Helper.IsCritical(0),
                        criticalBonus = 0,
                        hitPoint = nearbyObjects.transform.position,
                        hitDirection = direction
                    });
                }
            }
        }

        void ExplodeSound()
        {
            if (explodeClip != null)
                AudioManager.instance.PlaySound(explodeClip, transform.position);
            else
                throw new NotImplementedException("EnemyDeathFatBlowEffect: не прикреплён аудио файл");
        }

        StartCoroutine(Timer());
    }
}