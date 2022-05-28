using Interface;
using Pools;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileType projectileType;
    [SerializeField] private float speed = 10;
    [SerializeField] private float damage = 1;
    [SerializeField] private float criticalChance = 10;
    [SerializeField] private float criticalBonus = 100;
    [SerializeField] private float timeToDestroy = 2;

    private TrailRenderer _trailRenderer;
    private BoxCollider _boxCollider;
    private float _timer;
    private int _currentPenetrate;

    private void Awake()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
        _boxCollider = GetComponent<BoxCollider>();
        _trailRenderer.enabled = false;
    }

    private void OnEnable()
    {
        _currentPenetrate = ControllerManager.weaponController.PenetrateCount;
        
        _timer = Time.time + timeToDestroy;
        _trailRenderer.enabled = true;
        _boxCollider.enabled = true;
    }

    private void OnDisable()
    {
        _trailRenderer.enabled = false;
        _boxCollider.enabled = false;
    }

    private void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        transform.Translate(Vector3.forward * moveDistance);
        
        if(Time.time > _timer)
            BulletPool.Instance.ReturnToPool(this);
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

                if (_currentPenetrate <= 0)
                {
                    _boxCollider.enabled = false;
                    BulletPool.Instance.ReturnToPool(this);
                }
                    
            }
        }
    }

    public ProjectileType GetProjectileType => projectileType;
    
    public Projectile SetProjectileType(ProjectileType projectileType)
    {
        this.projectileType = projectileType;
        return this;
    }
    
    public Projectile SetPosition(Vector3 position)
    {
        transform.position = position;
        return this;
    }
    
    public Projectile SetRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
        return this;
    }

    public Projectile SetSpeed(float value)
    {
        speed = value;
        return this;
    }
    
    public Projectile SetDamage(float value)
    {
        damage = value;
        return this;
    }
    
    public Projectile SetCriticalChance(float value)
    {
        criticalChance = value;
        return this;
    }
    
    public Projectile SetCriticalBonus(float value)
    {
        criticalBonus = value;
        return this;
    }
}

public enum ProjectileType
{
    BULLET,
    ROCKET
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
