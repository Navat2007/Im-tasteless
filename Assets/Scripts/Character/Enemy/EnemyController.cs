using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(AnimationController))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(TargetSystem))]
public sealed class EnemyController : MonoBehaviour
{
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
    private List<Transform> _teleportPoints;
    private float randomPointRadius = 3f;

    private Vector3 _testMovePoint;
    private Vector3 _testTeleportPoint;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _animationController = GetComponent<AnimationController>();
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _camera = Camera.main;
        _collider = GetComponent<Collider>();
        _teleportPoints = ControllerManager.player.GetTeleportPoints;
    }

    private void OnDisable()
    {
        _enemy.GetEnemyTargetSystem.OnTargetChange -= OnTargetChange;
    }

    private void OnDrawGizmos()
    {
        if (_testMovePoint != Vector3.zero)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_testMovePoint, .1f);
        }

        if (_testTeleportPoint != Vector3.zero)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(_testTeleportPoint, .3f);
        }
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
        Vector3 RandomPointOnXZCircle(Vector3 center, float radius)
        {
            float angle = UnityEngine.Random.Range(0, 2f * Mathf.PI);
            return center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
        }

        Vector3? GetRandomPoint()
        {
            Vector3 randomPoint = Vector3.zero;

            NavMeshHit navMeshHit;
            Vector3 randomPointOnCircle = UnityEngine.Random.insideUnitCircle;

            NavMesh.SamplePosition(
                RandomPointOnXZCircle(target.transform.position, randomPointRadius),
                out navMeshHit,
                randomPointRadius,
                NavMesh.AllAreas);
            randomPoint = navMeshHit.position;
            randomPoint.y = 0;

            _navMeshAgent.CalculatePath(randomPoint, _navMeshPath);

            if ((randomPoint.x is > -10000 and < 10000) &&
                _navMeshPath.status == NavMeshPathStatus.PathComplete &&
                !NavMesh.Raycast(target.transform.position, randomPoint, out navMeshHit, NavMesh.AllAreas))
            {
                _testMovePoint = randomPoint;
                return randomPoint;
            }

            return null;
        }

        Vector3? GetRandomPointAtPlayerTeleportPoints()
        {
            Vector3 randomPoint = Vector3.zero;

            Random random = new Random();
            var point = _teleportPoints[random.Next(0, _teleportPoints.Count - 1)];

            randomPoint = point.position;

            _navMeshAgent.CalculatePath(randomPoint, _navMeshPath);

            if (_navMeshPath.status == NavMeshPathStatus.PathComplete)
            {
                randomPoint.y = 0.3f;
                _testTeleportPoint = randomPoint;
                return randomPoint;
            }

            return null;
        }

        Vector3 movePosition = Vector3.zero;
        Vector3 prevMovePosition = Vector3.zero;

        while (ControllerManager.player != null)
        {
            if (target != null && _navMeshAgent != null && _navMeshAgent.enabled)
            {
                distance = Vector3.Distance(transform.position, target.transform.position);

                if (_navMeshAgent.angularSpeed != _enemy.TurnSpeed + _bonusTurnSpeed)
                    _navMeshAgent.angularSpeed = _enemy.TurnSpeed + _bonusTurnSpeed;

                if (_navMeshAgent.speed != _enemy.MoveSpeed + _bonusSpeed)
                    _navMeshAgent.speed = _enemy.MoveSpeed + _bonusSpeed;

                if (Vector3.Distance(transform.position, target.transform.position) > randomPointRadius)
                {
                    if (Vector3.Distance(movePosition, target.transform.position) > randomPointRadius)
                    {
                        var point = GetRandomPoint();

                        if (point != null)
                        {
                            movePosition = point.Value;
                            _navMeshAgent.isStopped = false;
                            _animationController.SetState(AnimationState.RUN);
                        }
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
                    var point = RandomPointOnXZCircle(target.transform.position, 40f);
                    _testTeleportPoint = point;
                    //
                    // if (point != null)
                    // {
                    //     transform.position = point.Value;
                    // }
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
                Debug.LogError(e.Message);
                _navMeshAgent.enabled = false;
                _navMeshAgent.enabled = true;
            }

            yield return new WaitForSeconds(1);
        }
    }
}