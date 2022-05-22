using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(AnimationController))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(TargetSystem))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private float timeUpdateSpeed = 0.1f;
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private CapsuleCollider capsuleCollider2;
    
    private Enemy _enemy;
    private AnimationController _animationController;
    private Rigidbody _rigidbody;
    private NavMeshAgent _navMeshAgent;

    private float _bonusSpeed;
    private float _minBonusSpeed = -2.75f;
    private float _bonusTurnSpeed;
    private float _nextTimeUpdateSpeed;

    private GameObject _target;
    private bool _isDead;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _animationController = GetComponent<AnimationController>();
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _navMeshAgent.speed = _enemy.MoveSpeed;
        _animationController.SetState(AnimationState.IDLE);
        
        _enemy.GetEnemyTargetSystem.OnTargetChange += OnTargetChange;
        _enemy.GetEnemyTargetSystem.OnTargetPositionChange += OnTargetPositionChange;
    }

    private void OnDestroy()
    {
        _enemy.GetEnemyTargetSystem.OnTargetChange -= OnTargetChange;
        _enemy.GetEnemyTargetSystem.OnTargetPositionChange -= OnTargetPositionChange;
    }

    private void Update()
    {
        try
        {
            if (Time.time > _nextTimeUpdateSpeed && _navMeshAgent.enabled)
            {
                _nextTimeUpdateSpeed = Time.time + timeUpdateSpeed;
                _navMeshAgent.angularSpeed = _enemy.TurnSpeed + _bonusTurnSpeed;
                _navMeshAgent.speed = _enemy.MoveSpeed + _bonusSpeed;

                if (_target != null && Vector3.Distance(_target.transform.position, _enemy.transform.position) <=
                    _navMeshAgent.stoppingDistance)
                {
                    _navMeshAgent.isStopped = true;
                    _animationController.SetState(AnimationState.IDLE);
                }
                else
                {
                    _navMeshAgent.isStopped = false;
                    _animationController.SetState(AnimationState.RUN);
                }
            }
        }
        catch (Exception e)
        {
            _navMeshAgent.enabled = false;
            _navMeshAgent.enabled = true;
        }
    }

    private void OnTargetChange(GameObject target)
    {
        _target = target;
    }
    
    private void OnTargetPositionChange(Vector3 position, GameObject target)
    {
        if(_navMeshAgent != null && _navMeshAgent.enabled)
            _navMeshAgent.SetDestination(position);
    }
    
    public void OnDeath()
    {
        _isDead = true;

        if (_navMeshAgent.enabled)
        {
            _navMeshAgent.isStopped = true;
            _navMeshAgent.enabled = false;
        }
        
        capsuleCollider.enabled = false;
        capsuleCollider2.enabled = true;
        
        _animationController.SetState(AnimationState.DIE);
    }
    
    public void AddSpeed(float value)
    {
        _bonusSpeed += value;

        if (_bonusSpeed < _minBonusSpeed)
            _bonusSpeed = _minBonusSpeed;
    }

    public float GetBonusSpeed()
    {
        return _bonusSpeed;
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

    public void StopNavMeshAgent()
    {
        if (_navMeshAgent.enabled)
        {
            _navMeshAgent.isStopped = true;
            _navMeshAgent.enabled = false;
            _animationController.SetState(AnimationState.IDLE);
        }
    }
    
    public void StartNavMeshAgent(float time)
    {
        StartCoroutine(Wait(time));
    }

    private IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        
        if (!_isDead)
        {
            _navMeshAgent.enabled = true;
            _navMeshAgent.isStopped = false;
        }
    }
}