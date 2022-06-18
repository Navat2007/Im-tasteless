using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(AnimationController))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(TargetSystem))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private float randomPointRadius = 10f;
    [SerializeField] private GameObject target;
    [SerializeField] private bool isDead;
    [SerializeField] private bool isWalking;
    [SerializeField] private float distance;

    private Enemy _enemy;
    private AnimationController _animationController;
    private Rigidbody _rigidbody;
    private NavMeshAgent _navMeshAgent;
    private NavMeshPath _navMeshPath;
    private Camera _camera;
    private Collider _collider;

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
        _camera = Camera.main;
        _collider = GetComponent<Collider>();
    }

    private void OnDisable()
    {
        _enemy.GetEnemyTargetSystem.OnTargetChange -= OnTargetChange;
    }

    private void OnTargetChange(GameObject target)
    {
        this.target = target;
    }

    public void Init()
    {
        isDead = false;
        isWalking = false;

        _navMeshAgent.enabled = true;
        _navMeshAgent.isStopped = false;

        _collider.enabled = true;
        _rigidbody.isKinematic = false;

        _animationController.Init();

        _navMeshPath = new NavMeshPath();
        _navMeshAgent.speed = _enemy.MoveSpeed;

        _enemy.GetEnemyTargetSystem.OnTargetChange += OnTargetChange;

        StartCoroutine(Move());
    }

    public void OnDeath()
    {
        isDead = true;
        
        if (_navMeshAgent.enabled)
        {
            _navMeshAgent.isStopped = true;
            _navMeshAgent.enabled = false;
        }

        _animationController.SetState(AnimationState.DIE);

        _rigidbody.isKinematic = true;
        _collider.enabled = false;
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
            _rigidbody.velocity = Vector3.zero;
            _navMeshAgent.isStopped = true;
            _navMeshAgent.enabled = false;
            _animationController.SetState(AnimationState.IDLE);
        }
    }

    public void StartNavMeshAgent(float time)
    {
        StartCoroutine(Wait(time));
    }

    public void SendImpulse(Vector3 direction)
    {
        _rigidbody.AddForce(direction, ForceMode.Impulse);
    }

    public void SendForce(Vector3 direction)
    {
        _rigidbody.AddForce(direction);
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

    private IEnumerator Move()
    {
        Vector3 GetRandomPoint()
        {
            int count = 0;
            bool isCorrectPoint = false;
            Vector3 randomPoint = Vector3.zero;

            while (!isCorrectPoint && count < 10000)
            {
                NavMeshHit navMeshHit;
                NavMesh.SamplePosition(
                    UnityEngine.Random.insideUnitSphere * randomPointRadius + target.transform.position, out navMeshHit,
                    randomPointRadius, NavMesh.AllAreas);
                randomPoint = navMeshHit.position;
                randomPoint.y = 0;

                if (randomPoint.x > -10000 && randomPoint.x < 10000)
                {
                    _navMeshAgent.CalculatePath(randomPoint, _navMeshPath);
                    if (_navMeshPath.status == NavMeshPathStatus.PathComplete &&
                        !NavMesh.Raycast(target.transform.position, randomPoint, out navMeshHit, NavMesh.AllAreas))
                        isCorrectPoint = true;
                }

                count++;
            }

            return randomPoint;
        }
        
        Vector3 GetRandomPointAtDistance(float maxDistance)
        {
            int count = 0;
            bool isCorrectPoint = false;
            Vector3 randomPoint = Vector3.zero;

            while (!isCorrectPoint && count < 10000)
            {
                var randomDirection = (UnityEngine.Random.insideUnitCircle * new Vector2(target.transform.position.x, target.transform.position.z)).normalized;
                var randomDistance = UnityEngine.Random.Range(maxDistance, maxDistance);
                var point = new Vector2(target.transform.position.x, target.transform.position.z) + randomDirection * randomDistance;

                randomPoint = new Vector3(point.x, 0, point.y);

                if (randomPoint.x > -10000 && randomPoint.x < 10000)
                {
                    _navMeshAgent.CalculatePath(randomPoint, _navMeshPath);
                    if (_navMeshPath.status == NavMeshPathStatus.PathComplete)
                        isCorrectPoint = true;
                }

                count++;
            }
            
            randomPoint.y = 0.03f;
            
            //Debug.Log($"Send zombie to point: {randomPoint}. Player position: {target.transform.position}");

            return randomPoint;
        }
        
        Vector3 movePosition = Vector3.zero;
        Vector3 prevMovePosition = Vector3.zero;

        while (ControllerManager.player != null)
        {
            if (target != null && _navMeshAgent != null && _navMeshAgent.enabled)
            {
                distance = Vector3.Distance(transform.position, target.transform.position);

                if(_navMeshAgent.angularSpeed != _enemy.TurnSpeed + _bonusTurnSpeed)
                    _navMeshAgent.angularSpeed = _enemy.TurnSpeed + _bonusTurnSpeed;
            
                if(_navMeshAgent.speed != _enemy.MoveSpeed + _bonusSpeed)
                    _navMeshAgent.speed = _enemy.MoveSpeed + _bonusSpeed;

                if (Vector3.Distance(transform.position, target.transform.position) > randomPointRadius)
                {
                    if (Vector3.Distance(movePosition, target.transform.position) > randomPointRadius)
                    {
                        movePosition = GetRandomPoint();
                        _navMeshAgent.isStopped = false;
                        _animationController.SetState(AnimationState.RUN);
                    }
                }
                else
                {
                    movePosition = target.transform.position;
                
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

                if (_navMeshAgent != null && _navMeshAgent.enabled && movePosition != prevMovePosition)
                {
                    prevMovePosition = movePosition;
                    _navMeshAgent.SetDestination(movePosition);  
                }
                
                if (distance >= 50)
                {
                    transform.position = GetRandomPointAtDistance(30);
                }
            }

            yield return new WaitForSeconds(0.1f);
        }

        if (ControllerManager.player == null && !isWalking && !isDead)
        {
            isWalking = true;
            _navMeshAgent.enabled = true;
            _navMeshAgent.isStopped = false;
            _navMeshAgent.speed = 1f;
            _animationController.SetState(AnimationState.WALK);
            StartCoroutine(TurnOnWalking());
        }
    }

    private IEnumerator TurnOnWalking()
    {
        void SetNewDestination()
        {
            System.Random random = new System.Random();

            var position = new Vector3(_camera.transform.localPosition.x + random.Next(-30, 30), 0,
                (_camera.transform.localPosition.z + 10) + random.Next(-30, 30));

            if (_navMeshAgent != null)
                _navMeshAgent.SetDestination(position);
        }

        yield return new WaitForSeconds(1);

        float nextDestinationTime = 0;

        while (true)
        {
            if (Time.time > nextDestinationTime)
            {
                System.Random random = new System.Random();
                nextDestinationTime = Time.time + random.Next(30, 60);
                SetNewDestination();
            }

            try
            {
                if (_navMeshAgent != null & _navMeshAgent.remainingDistance <= 1f)
                    _animationController.SetState(AnimationState.IDLE);
                else if (_navMeshAgent != null & _navMeshAgent.remainingDistance > 1f)
                    _animationController.SetState(AnimationState.WALK);
            }
            catch (Exception e)
            {
                _navMeshAgent.enabled = false;
                _navMeshAgent.enabled = true;
            }

            yield return new WaitForSeconds(1);
        }
    }
}