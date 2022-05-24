using System;
using System.Collections;
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
    
    private bool _isExploded;
    
    private void OnEnable()
    {
        _isExploded = false;
        healthSystem.OnDeath += OnDeath;
    }

    private void OnDisable()
    {
        healthSystem.OnDeath -= OnDeath;
    }
    
    private void Update()
    {
        if(_isExploded)
            transform.localScale += Vector3.one * Time.deltaTime * growSpeed;
    }

    protected override void OnDeath(ProjectileHitInfo projectileHitInfo)
    {
        if (_isExploded)
            return;

        _isExploded = true;
        
        healthSystem.enabled = false;
        
        if(gameObject.TryGetComponent(out EnemyPowerSkin skin))
        {
            skin.Switch();
            
            timer += 1;
            radius += 3;
            force *= 2;
            damage *= 3;
        }
        
        if(gameObject.TryGetComponent(out EnemyController controller))
        {
            controller.ResetBonusSpeed();
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

            AddForce();
            ExplodeSound();
            
            enemy.GetEnemyController.OnDeath();
            enemy.Die();
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