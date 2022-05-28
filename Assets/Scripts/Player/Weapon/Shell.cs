using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shell : MonoBehaviour
{
    [SerializeField] private float destroyTime = 4;
    [SerializeField] private float forceMin;
    [SerializeField] private float forceMax;
    
    private Rigidbody _rigidbody;
    private ShellType _shellType;
    private float timer;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        timer = Time.time + destroyTime;
        
        float force = Random.Range(forceMin, forceMax);
        _rigidbody.AddForce(transform.right * force);
        _rigidbody.AddTorque(Random.insideUnitSphere * force);
    }

    private void Update()
    {
        if(Time.time > timer)
            ShellPool.Instance.ReturnToPool(this);
    }
    
    public Shell SetPosition(Vector3 position)
    {
        transform.position = position;
        return this;
    }
    
    public Shell SetRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
        return this;
    }

    public ShellType GetShellType => _shellType;

    public void SetShellType(ShellType shellType)
    {
        _shellType = shellType;
    }
}

public enum ShellType
{
    PISTOL,
    SHOTGUN,
    RIFLE
}
