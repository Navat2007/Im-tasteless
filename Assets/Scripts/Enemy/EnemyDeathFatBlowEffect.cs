using System;
using System.Collections;
using System.Collections.Generic;
using Interface;
using UnityEngine;

public class EnemyDeathFatBlowEffect : EnemyDeathEffect
{
    [SerializeField] private float timer = 2;
    [SerializeField] private float radius = 5;
    [SerializeField] private float force = 400;
    [SerializeField] private float damage = 10;
    [SerializeField] private float upwardsModifier = 0;
    [SerializeField] private ForceMode forceMode;
    [SerializeField] private float stunTime = 1;
    [SerializeField] private AudioClip explodeClip;

    [Header("Скорость расширения")]
    [SerializeField] private float growSpeed = 0.4f;

    private List<ParticleSystem> _particleSystems = new();
    private bool _isExploded;
    private float _timer;
    private bool _spawned;

    private void Update()
    {
        if(_isExploded)
            transform.localScale += Vector3.one * Time.deltaTime * growSpeed;
        
        if (_spawned && Time.time > _timer)
        {
            foreach (var item in _particleSystems)
            {
                DeathEffectPool.Instance.ReturnToPool(item);
            }
        }
    }

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
            List<Vector3> vectors = new List<Vector3>
            {
                Vector3.forward,
                Vector3.back,
                Vector3.right,
                Vector3.left,
                Vector3.forward - Vector3.left,
                Vector3.forward - Vector3.right,
                Vector3.back - Vector3.left,
                Vector3.back - Vector3.right
            };

            foreach (var item in vectors)
            {
                var pSystem = DeathEffectPool.Instance.Get();
                pSystem.transform.position = transform.position;
                pSystem.transform.rotation = Quaternion.FromToRotation(item, projectileHitInfo.hitDirection);
                pSystem.gameObject.SetActive(true);
                _particleSystems.Add(pSystem);
            }

            _timer = Time.time + _particleSystems[0].main.startLifetime.constantMax;
            _spawned = true;
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