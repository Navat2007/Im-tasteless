using System;
using System.Collections;
using UnityEngine;

public class TargetSystem : MonoBehaviour
{
    public event Action<GameObject> OnTargetChange;
    public event Action<Vector3, GameObject> OnTargetPositionChange;

    [SerializeField] private float timeBetweenSearchTarget = 0.5f;
    
    private GameObject _target;

    private void Start()
    {
        StartCoroutine(FindTarget());
    }

    private void SetTarget(GameObject target)
    {
        _target = target;
        OnTargetChange?.Invoke(_target);
        
        StartCoroutine(FindTargetPosition());
    }

    private IEnumerator FindTarget()
    {
        while (_target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
                
            if(player != null)
                SetTarget(player);

            yield return new WaitForSeconds(timeBetweenSearchTarget);
        }
    }
    
    private IEnumerator FindTargetPosition()
    {
        while (_target != null && _target.activeSelf)
        {
            Vector3 position = new Vector3(_target.transform.position.x, 0, _target.transform.position.z);
            OnTargetPositionChange?.Invoke(position, _target);

            yield return new WaitForSeconds(timeBetweenSearchTarget);
        }
        
        StartCoroutine(FindTarget());
    }

    public GameObject GetTarget()
    {
        return _target;
    }
}
