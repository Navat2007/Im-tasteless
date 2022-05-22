using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DebugNavMeshAgent : MonoBehaviour
{
    [SerializeField] private bool showVelocity;
    [SerializeField] private bool showDesiredVelocity;
    [SerializeField] private bool showPath;
    
    private NavMeshAgent _navMeshAgent;
    
    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void OnDrawGizmos()
    {
        if (_navMeshAgent != null && _navMeshAgent.enabled)
        {
            if (showVelocity)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, transform.position + _navMeshAgent.velocity);
            }
        
            if (showDesiredVelocity)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + _navMeshAgent.desiredVelocity);
            }
        
            if (showPath)
            {
                var agentPath = _navMeshAgent.path;
                Vector3 prevCorner = transform.position;

                foreach (var corner in agentPath.corners)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(prevCorner, corner);
                    Gizmos.DrawSphere(corner, 0.1f);
                    prevCorner = corner;
                }
            }
        }
    }
}
