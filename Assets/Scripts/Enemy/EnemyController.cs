using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(TargetSystem))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private float timeUpdateSpeed = 0.2f;
    
    private Enemy _enemy;
    private Rigidbody _rigidbody;
    private NavMeshAgent _navMeshAgent;
    private TargetSystem _targetSystem;

    private float _bonusSpeed;
    private float _bonusTurnSpeed;
    private float _nextTimeUpdateSpeed;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _targetSystem = GetComponent<TargetSystem>();
    }

    private void Start()
    {
        _navMeshAgent.speed = _enemy.MoveSpeed;
    }

    private void OnEnable()
    {
        _targetSystem.OnTargetPositionChange += OnTargetPositionChange;
    }

    private void OnDisable()
    {
        _targetSystem.OnTargetPositionChange -= OnTargetPositionChange;
    }

    private void Update()
    {
        if (Time.time > _nextTimeUpdateSpeed)
        {
            _nextTimeUpdateSpeed = Time.time + timeUpdateSpeed;
            _navMeshAgent.angularSpeed = _enemy.TurnSpeed + _bonusTurnSpeed;
            _navMeshAgent.speed = _enemy.MoveSpeed + _bonusSpeed;
        }
    }

    private void OnTargetChange(GameObject target)
    {
        _navMeshAgent.SetDestination(target.transform.position);
    }
    
    private void OnTargetPositionChange(Vector3 position, GameObject target)
    {
        _navMeshAgent.SetDestination(position);
    }
    
    public void AddSpeed(float value)
    {
        _bonusSpeed += value;
    }
    
    public void AddTurnSpeed(float value)
    {
        _bonusTurnSpeed += value;
    }

    public void ResetBonusSpeed()
    {
        _bonusSpeed = 0;
        _bonusTurnSpeed = 0;
    }
}
