using System;
using System.Collections;
using Interface;
using UnityEngine;

public class EnemyDeathFatBlowEffect : EnemyDeathEffect
{
    [SerializeField] private float timer = 2;
    [SerializeField] private float radius = 5;
    [SerializeField] private float force = 5;
    [SerializeField] private float damage = 10;
    [SerializeField] private AudioClip explodeClip;

    private bool _isExploded;

    protected override void OnDeath(ProjectileHitInfo projectileHitInfo)
    {
        if (_isExploded)
            return;

        _isExploded = true;
        base.OnDeath(projectileHitInfo);
        
        IEnumerator Timer()
        {
            float timeToExplode = Time.time + timer;

            while (Time.time < timeToExplode)
            {
                transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
                
                float intensity = Mathf.PingPong (Time.time, .1f) / .1f;
                meshRenderer.material.EnableKeyword("_EMISSION");
                meshRenderer.material.SetColor("_EmissionColor", Color.red * intensity);
                yield return new WaitForSeconds(.1f);
            }
            
            ShowEffect();
            AddForce();
            ExplodeSound();
            
            Destroy(gameObject);
        }

        void ShowEffect()
        {
            var deathEffectRenderer = deathEffectParticlePrefab.GetComponent<ParticleSystemRenderer>();
            deathEffectRenderer.material = meshRenderer.material;

            var deathEffectGameObject = Instantiate(
                deathEffectParticlePrefab.gameObject,
                projectileHitInfo.hitPoint,
                Quaternion.FromToRotation(Vector3.forward, projectileHitInfo.hitDirection));

            var deathEffectGameObject2 = Instantiate(
                deathEffectParticlePrefab.gameObject,
                projectileHitInfo.hitPoint,
                Quaternion.FromToRotation(Vector3.back, projectileHitInfo.hitDirection));

            var deathEffectGameObject3 = Instantiate(
                deathEffectParticlePrefab.gameObject,
                projectileHitInfo.hitPoint,
                Quaternion.FromToRotation(Vector3.right, projectileHitInfo.hitDirection));

            var deathEffectGameObject4 = Instantiate(
                deathEffectParticlePrefab.gameObject,
                projectileHitInfo.hitPoint,
                Quaternion.FromToRotation(Vector3.left, projectileHitInfo.hitDirection));

            var deathEffectGameObject5 = Instantiate(
                deathEffectParticlePrefab.gameObject,
                projectileHitInfo.hitPoint,
                Quaternion.FromToRotation(Vector3.forward - Vector3.left, projectileHitInfo.hitDirection));

            var deathEffectGameObject6 = Instantiate(
                deathEffectParticlePrefab.gameObject,
                projectileHitInfo.hitPoint,
                Quaternion.FromToRotation(Vector3.forward - Vector3.right, projectileHitInfo.hitDirection));

            var deathEffectGameObject7 = Instantiate(
                deathEffectParticlePrefab.gameObject,
                projectileHitInfo.hitPoint,
                Quaternion.FromToRotation(Vector3.back - Vector3.left, projectileHitInfo.hitDirection));

            var deathEffectGameObject8 = Instantiate(
                deathEffectParticlePrefab.gameObject,
                projectileHitInfo.hitPoint,
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
                    rigidbody.AddExplosionForce(force, transform.position, radius);
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
                        isCritical = Helper.GetCriticalChance(0),
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