using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFastController : MonoBehaviour
{
    [SerializeField] private float movePercent = -60;
    [SerializeField] private float stuckMaxTimer = 2f;
    [SerializeField] private float stuckPercent = 10f;
    
    private Enemy _enemy;
    private EnemyController _enemyController;
    private HealthSystem _healthSystem;

    private bool _isSlowed;
    private float _stuckTimer;
    private float _bonusSpeed;
    private float _prevCount;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _enemyController = GetComponent<EnemyController>();
        _healthSystem = GetComponent<HealthSystem>();
        
        _healthSystem.OnTakeDamage += OnTakeDamage;

        _bonusSpeed = _enemy.MoveSpeed / 100 * movePercent;
    }
    
    private void Update()
    {
        if (Time.time > _stuckTimer && _isSlowed)
        {
            _enemyController.AddSpeed(_bonusSpeed);
            _isSlowed = false;
        }
    }

    private void OnTakeDamage(float damage, float health, float maxHealth)
    {
        var percent = 100 - (health * 100 / maxHealth);
        var count = Mathf.Round(percent / stuckPercent);

        if (count > _prevCount && !_isSlowed)
        {
            _isSlowed = true;
            _enemyController.AddSpeed(-_bonusSpeed);
            _stuckTimer = Time.time + stuckMaxTimer;
            _prevCount = count;
        }
        else if (count > _prevCount && _isSlowed)
        {
            _stuckTimer = Time.time + stuckMaxTimer;
            _prevCount = count;
        }

    }
}
