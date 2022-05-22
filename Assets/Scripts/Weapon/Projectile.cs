using Interface;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    [SerializeField] private float damage = 1;
    [SerializeField] private float criticalChance = 10;
    [SerializeField] private float criticalBonus = 100;

    private float _timeToDestroy = 3;
    private int _currentPenetrate;

    private void Start()
    {
        _currentPenetrate = ControllerManager.weaponController.PenetrateCount;
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
            if (healthSystem.enabled)
            {
                var damageToAdd = enemy.GetComponent<Enemy>() != null && ControllerManager.weaponController != null && Helper.IsCritical(ControllerManager.weaponController.KillChance) ? healthSystem.MaxHealth : damage;
                
                healthSystem.TakeDamage(new ProjectileHitInfo
                {
                    damage =  damageToAdd,
                    isCritical = Helper.IsCritical(criticalChance),
                    criticalBonus = criticalBonus,
                    hitPoint = other.ClosestPoint(transform.position),
                    hitDirection = transform.forward
                });
                
                _currentPenetrate--;
        
                if(_currentPenetrate == 0)
                    Destroy(gameObject);
            }
        }
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

public struct ProjectileHitInfo
{
    public float damage;
    public bool isCritical;
    public float criticalBonus;
    public Vector3 hitPoint;
    public Vector3 hitDirection;

    public void MakeDamageCritical()
    {
        damage += damage / 100 * criticalBonus;
    }
}
