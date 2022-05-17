using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TransportPlane : MonoBehaviour
{
    public event Action OnSpawn;
    
    [SerializeField] private float speed = 30;

    private Vector3 _startPosition;
    private bool _spawned;

    private void Awake()
    {
        _startPosition = transform.position;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (!_spawned && transform.position.x >= -30 && transform.position.x <= -20)
        {
            SpawnGoods();
        }

        if (transform.position.x > 400)
        {
            Reset();
        }
    }

    private void SpawnGoods()
    {
        _spawned = true;
        OnSpawn?.Invoke();
    }

    public void Reset()
    {
        gameObject.SetActive(false);
        transform.position = _startPosition;
        _spawned = false;
    }
}
