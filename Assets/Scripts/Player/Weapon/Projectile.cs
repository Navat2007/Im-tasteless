using Interface;
using Pools;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileType projectileType;
    [SerializeField] private float speed = 10;
    [SerializeField] private float damage = 1;
    [SerializeField] private float periodDamage = 0;
    [SerializeField] private float periodDuration = 0;
    [SerializeField] private float periodTick = 0;
    [SerializeField] private float criticalChance = 10;
    [SerializeField] private float criticalBonus = 100;
    [SerializeField] private float timeToDestroy = 2;

    private TrailRenderer _trailRenderer;
    private BoxCollider _boxCollider;
    private float _timer;
    private int _currentPenetrate;
    private bool _knockBack;

    private void Awake()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
        _boxCollider = GetComponent<BoxCollider>();
        _trailRenderer.enabled = false;
    }

    private void OnEnable()
    {
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

        if (Time.time > _timer)
            BulletPool.Instance.ReturnToPool(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.gameObject;

        if (enemy.GetComponent<IDamageable>() != null && enemy.TryGetComponent(out HealthSystem healthSystem))
        {
            if (healthSystem.enabled)
            {
                //TODO добавить значок убийства с 1 выстрела
                var damageToAdd =
                    enemy.GetComponent<Enemy>() != null && ControllerManager.weaponController != null &&
                    Helper.IsCritical(ControllerManager.weaponController.GetKillChance)
                        ? healthSystem.MaxHealth
                        : damage;

                healthSystem.TakeDamage(new ProjectileHitInfo
                {
                    damage = damageToAdd,
                    periodDamageStruct = new PeriodDamageStruct
                    {
                        source = PeriodDamageSource.PROJECTILE,
                        damage = periodDamage,
                        duration = periodDuration,
                        tick = periodTick
                    },
                    isCritical = Helper.IsCritical(criticalChance),
                    criticalBonus = criticalBonus,
                    hitPoint = other.ClosestPoint(transform.position),
                    hitDirection = transform.forward
                });

                if (_knockBack && enemy.TryGetComponent(out EnemyController controller))
                {
                    controller.SendForce(transform.forward.normalized * 50);
                }

                if (_currentPenetrate <= 0)
                {
                    _boxCollider.enabled = false;
                    BulletPool.Instance.ReturnToPool(this);
                }
                
                _currentPenetrate--;
                damage /= 2;
            }
        }
    }

    public ProjectileType GetProjectileType => projectileType;

    #region Setters

    public Projectile SetProjectileType(ProjectileType type)
    {
        projectileType = type;
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
    
    public Projectile SetPeriodDamage(float value)
    {
        periodDamage = value;
        return this;
    }
    
    public Projectile SetPeriodDuration(float value)
    {
        periodDuration = value;
        return this;
    }
    
    public Projectile SetPeriodTick(float value)
    {
        periodTick = value;
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
    
    public Projectile SetKnockBack(bool value)
    {
        _knockBack = value;
        return this;
    }

    public Projectile SetPenetrateCount(int value)
    {
        _currentPenetrate = value;
        return this;
    }

    #endregion

}

public enum ProjectileType
{
    BULLET,
    ROCKET
}

public struct ProjectileHitInfo
{
    public float damage;
    public PeriodDamageStruct periodDamageStruct;
    public bool isCritical;
    public float criticalBonus;
    public Vector3 hitPoint;
    public Vector3 hitDirection;

    public void MakeDamageCritical()
    {
        damage += damage / 100 * criticalBonus;
    }
    
    public void MakePeriodDamageCritical()
    {
        periodDamageStruct.damage += periodDamageStruct.damage / 100 * criticalBonus;
    }
}