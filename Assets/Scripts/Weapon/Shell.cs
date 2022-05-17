using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField] private float destroyTime = 4;
    [SerializeField] private float forceMin;
    [SerializeField] private float forceMax;
    
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        float force = Random.Range(forceMin, forceMax);
        _rigidbody.AddForce(transform.right * force);
        _rigidbody.AddTorque(Random.insideUnitSphere * force);
        
        Destroy(gameObject, destroyTime);
    }
}
