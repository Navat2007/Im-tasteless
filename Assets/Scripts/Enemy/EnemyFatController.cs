using System;
using UnityEngine;

public class EnemyFatController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.2f;
    [SerializeField] private float turnSpeed = -3.0f;
    
    private EnemyController _enemyController;
    private HealthSystem _healthSystem;

    private float _prevCount;

    private void Awake()
    {
        _enemyController = GetComponent<EnemyController>();
        _healthSystem = GetComponent<HealthSystem>();
        
        _healthSystem.OnHealthChange += OnHealthChange;
    }

    private void OnHealthChange(float health, float maxHealth)
    {
        var percent = 100 - (health * 100 / maxHealth);
        var count = Mathf.Round(percent / 5);

        if (count > _prevCount)
        {
            _enemyController.SetSpeed(moveSpeed * (count - _prevCount));
            _enemyController.SetTurnSpeed(turnSpeed * (count - _prevCount));
            _prevCount = count;
        }
        
        print($"Percent: {percent}");
        print($"Module: {Math.Round(percent / 5)}");
    }
}
