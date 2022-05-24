using System;
using System.Collections;
using UnityEngine;

public class TargetSystem : MonoBehaviour
{
    public event Action<GameObject> OnTargetChange;
    public event Action<Vector3, GameObject> OnTargetPositionChange;

    [SerializeField] private float timeBetweenSearchTarget = 0.5f;
    
    [SerializeField] private GameObject target;

    private void Start()
    {
        StartCoroutine(FindTarget());
    }

    private void SetTarget(GameObject target)
    {
        this.target = target;
        OnTargetChange?.Invoke(this.target);
        
        StartCoroutine(FindTargetPosition());
    }

    private IEnumerator FindTarget()
    {
        while (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
                
            if(player != null)
                SetTarget(player);

            yield return new WaitForSeconds(timeBetweenSearchTarget);
        }
    }
    
    private IEnumerator FindTargetPosition()
    {
        while (target != null && target.activeSelf)
        {
            Vector3 position = new Vector3(target.transform.position.x, 0, target.transform.position.z);
            OnTargetPositionChange?.Invoke(position, target);

            yield return new WaitForSeconds(timeBetweenSearchTarget);
        }
        
        StartCoroutine(FindTarget());
    }

    public GameObject GetTarget()
    {
        return target;
    }
}
