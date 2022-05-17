using Interface;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    [SerializeField] private float damage = 1;
    [SerializeField] private float criticalChance = 10;
    [SerializeField] private float criticalBonus = 2;
    [SerializeField] private LayerMask collisionMask;

    private float _timeToDestroy = 3;
    private float _skinWidth = .1f;

    private void Start()
    {
        Destroy(gameObject, _timeToDestroy);
    }

    private void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        transform.Translate(Vector3.forward * moveDistance);
    }

    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.gameObject;
        
        if (enemy.GetComponent<IDamageable>() != null && enemy.TryGetComponent(out HealthSystem healthSystem))
        {
            healthSystem.TakeHit(damage, Helper.GetCriticalChance(criticalChance), other.ClosestPoint(transform.position), transform.forward);
        }
        
        Destroy(gameObject);
    }

    public void SetSpeed(float value)
    {
        speed = value;
    }
    
    public void SetDamage(float value)
    {
        damage = value;
    }
    
    public void SetCriticalChance(float value)
    {
        criticalChance = value;
    }
    
    public void SetCriticalBonus(float value)
    {
        criticalBonus = value;
    }
}
