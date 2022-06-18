using System;
using System.Collections;
using UnityEngine;
using Random = System.Random;

public class TargetSystem : MonoBehaviour
{
    public event Action<GameObject> OnTargetChange;
    public event Action<Vector3, GameObject> OnTargetPositionChange;

    [SerializeField] private float timeBetweenSearchTarget = 1f;
    
    [SerializeField] private GameObject target;

    private GameObject _player;
    private Vector3 _prevPlayerPosition;

    private void SetTarget(GameObject target)
    {
        this.target = target;
        OnTargetChange?.Invoke(this.target);
    }

    private void FindTarget()
    {
        if(_player != null && target == null)
            SetTarget(_player);
    }

    public void Init()
    {
        target = null;

        if (ControllerManager.player != null)
        {
            _player = ControllerManager.player.gameObject;
            InvokeRepeating(nameof(FindTarget), 0, timeBetweenSearchTarget);
        }
    }

    public GameObject GetTarget()
    {
        return target;
    }
}
