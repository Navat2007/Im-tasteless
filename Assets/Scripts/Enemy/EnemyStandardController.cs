using UnityEngine;

public class EnemyStandardController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float stuckMaxTimer = 1f;
    [SerializeField] private float stuckMaxCount = 4f;
    
    private EnemyController _enemyController;
    private HealthSystem _healthSystem;

    private float _stuckCount;
    private float _stuckTimer;

    private void Awake()
    {
        _enemyController = GetComponent<EnemyController>();
        _healthSystem = GetComponent<HealthSystem>();
        
        _healthSystem.OnTakeDamage += OnTakeDamage;
    }

    private void Update()
    {
        if (Time.time > _stuckTimer && _stuckCount > 0)
        {
            _enemyController.AddSpeed(-moveSpeed * _stuckCount);
            _stuckCount = 0;
        }
    }

    private void OnTakeDamage(float damage, float health, float maxHealth)
    {
        if (_stuckCount < stuckMaxCount)
        {
            _stuckCount++;
            _enemyController.AddSpeed(moveSpeed);
        }
        
        _stuckTimer = Time.time + stuckMaxTimer;
    }
}
