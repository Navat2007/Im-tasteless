using System;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(TargetSystem))]
[RequireComponent(typeof(AnimationController))]
public class EnemyAttackController : MonoBehaviour
{
    [SerializeField] private GameObject attackSector;
    
    private Enemy _enemy;
    private AnimationController _animationController;

    private float _nextAttackTime;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _animationController = GetComponent<AnimationController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enabled && other.CompareTag("Player"))
        {
            if (Time.time > _nextAttackTime)
            {
                Attack(other.gameObject); 
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (enabled && other.CompareTag("Player"))
        {
            if (Time.time > _nextAttackTime)
            {
                Attack(other.gameObject); 
            }
        }
    }
    
    private void Attack(GameObject target)
    {
        _nextAttackTime = Time.time + _enemy.MsBetweenAttack / 1000;

        if (target.gameObject.TryGetComponent(out HealthSystem healthSystem))
        {
            healthSystem.TakeDamage(new ProjectileHitInfo
            {
                damage = _enemy.Damage,
                isCritical = Helper.IsCritical(_enemy.CriticalChance),
                criticalBonus = _enemy.CriticalBonus,
                hitPoint = transform.position,
                hitDirection = transform.forward
            });
        }

        _animationController.SetState(AnimationState.ATTACK);
    }

    public void Init()
    {
        
    }
}
