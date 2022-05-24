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
    [SerializeField] private GameObject target;
    [SerializeField] private bool isDead;
    [SerializeField] private bool isWalking;
    
    private Enemy _enemy;
    private AnimationController _animationController;
    private Rigidbody _rigidbody;
    private NavMeshAgent _navMeshAgent;

    private float _bonusSpeed;
    private float _minBonusSpeed = -2.75f;
    private float _bonusTurnSpeed;
    private float _nextTimeUpdateSpeed;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _animationController = GetComponent<AnimationController>();
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    private void OnDisable()
    {
        _enemy.GetEnemyTargetSystem.OnTargetChange -= OnTargetChange;
        _enemy.GetEnemyTargetSystem.OnTargetPositionChange -= OnTargetPositionChange;
    }

    private void Update()
    {
        try
        {
            if(ControllerManager.player == null && !isWalking && !isDead)
            {
                isWalking = true;
                _navMeshAgent.enabled = true;
                _navMeshAgent.isStopped = false;
                _navMeshAgent.speed = 1f;
                _animationController.SetState(AnimationState.WALK);
                StartCoroutine(TurnOnWalking());
            }
            
            if (Time.time > _nextTimeUpdateSpeed && _navMeshAgent.enabled && !isWalking && !isDead)
            {
                _nextTimeUpdateSpeed = Time.time + timeUpdateSpeed;
                _navMeshAgent.angularSpeed = _enemy.TurnSpeed + _bonusTurnSpeed;
                _navMeshAgent.speed = _enemy.MoveSpeed + _bonusSpeed;

                if (target != null && Vector3.Distance(target.transform.position, _enemy.transform.position) <=
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
        this.target = target;
    }
    
    private void OnTargetPositionChange(Vector3 position, GameObject target)
    {
        if(_navMeshAgent != null && _navMeshAgent.enabled)
            _navMeshAgent.SetDestination(position);
    }
    
    public void Init()
    {
        isDead = false;
        isWalking = false;
        
        _navMeshAgent.enabled = true;
        _navMeshAgent.isStopped = false;
        
        capsuleCollider.enabled = true;
        capsuleCollider2.enabled = false;
        
        _animationController.Init();
        
        _navMeshAgent.speed = _enemy.MoveSpeed;
        
        _enemy.GetEnemyTargetSystem.OnTargetChange += OnTargetChange;
        _enemy.GetEnemyTargetSystem.OnTargetPositionChange += OnTargetPositionChange;
    }
    
    public void OnDeath()
    {
        isDead = true;

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
        
        if (!isDead)
        {
            _navMeshAgent.enabled = true;
            _navMeshAgent.isStopped = false;
        }
    }

    private IEnumerator TurnOnWalking()
    {
        void SetNewDestination()
        {
            System.Random random = new System.Random();

            var position = new Vector3(Camera.main.transform.localPosition.x + random.Next(-30, 30), 0, (Camera.main.transform.localPosition.z + 10) + random.Next(-30, 30));
            
            if(_navMeshAgent != null)
                _navMeshAgent.SetDestination(position);
        }
        
        yield return new WaitForSeconds(1);
        SetNewDestination();

        float timer = 0;
        
        while (true)
        {
            timer += 1;

            System.Random random = new System.Random();
            if (timer >= random.Next(30, 60))
            {
                timer = 0;
                SetNewDestination();
            }
            
            if(_navMeshAgent != null & _navMeshAgent.remainingDistance <= 1f)
                _animationController.SetState(AnimationState.IDLE);
            else if(_navMeshAgent != null & _navMeshAgent.remainingDistance > 1f)
                _animationController.SetState(AnimationState.WALK);

            yield return new WaitForSeconds(1);
        }
    }
}