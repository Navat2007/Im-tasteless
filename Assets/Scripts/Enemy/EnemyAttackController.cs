using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(TargetSystem))]
[RequireComponent(typeof(Animator))]
public class EnemyAttackController : MonoBehaviour
{
    private Enemy _enemy;
    private TargetSystem _targetSystem;
    private Animator _animator;

    private float _nextAttackTime;
    
    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _targetSystem = GetComponent<TargetSystem>();
        _animator = GetComponent<Animator>();
    }
    
    private void OnEnable()
    {
        _targetSystem.OnTargetPositionChange += OnTargetPositionChange;
    }

    private void OnDisable()
    {
        _targetSystem.OnTargetPositionChange -= OnTargetPositionChange;
    }
    
    private void OnTargetPositionChange(Vector3 position, GameObject target)
    {
        if (Time.time > _nextAttackTime)
        {
            float sqrDistanceToTarget = (position - transform.position).sqrMagnitude;

            if (target.activeSelf && sqrDistanceToTarget < Mathf.Pow(_enemy.AttackDistance, 2))
                Attack(target);
        }
    }

    private void Attack(GameObject target)
    {
        _nextAttackTime = Time.time + _enemy.MsBetweenAttack / 1000;

        if (target.gameObject.TryGetComponent(out HealthSystem healthSystem))
        {
            healthSystem.TakeDamage(_enemy.Damage, false);
        }
        
        _animator?.SetTrigger("Attack");
        
    }
}
