using System;
using Interface;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(HealthSystem))]
public class Enemy : MonoBehaviour, IHealth, IDamageable
{
    public event Action OnDeath;
    
    [field: SerializeField] public ZombieType ZombieType { get; private set; }
    [field: SerializeField] public Renderer Renderer { get; private set; }
    [field: SerializeField] public Renderer[] SkinRendererList { get; private set; }
    
    [field: Header("Стандартные параметры")]
    [field: SerializeField] public float Health { get; set; }
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float CriticalChance { get; private set; }
    [field: SerializeField] public float CriticalBonus { get; private set; }
    [field: SerializeField] public float AttackDistance { get; private set; }
    [field: SerializeField] public float MsBetweenAttack { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public float TurnSpeed { get; private set; }
    [field: SerializeField] public int XpOnDeath { get; private set; } = 1;
    [field: SerializeField] public Color Color { get; private set; }
    [field: SerializeField] public Vector3 Size { get; private set; }

    private TargetSystem _targetSystem;
    private HealthSystem _healthSystem;
    private EnemyAttackController _attackController;
    private EnemyController _enemyController;
    private EnemyDeathEffect _enemyDeathEffect;

    private Color _baseColor;

    private bool _isPower;

    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();
        _targetSystem = GetComponent<TargetSystem>();
        _attackController = GetComponent<EnemyAttackController>();
        _enemyController = GetComponent<EnemyController>();
        _enemyDeathEffect = GetComponent<EnemyDeathEffect>();

        _isPower = GetComponent<EnemyPowerSkin>() != null;
        
        if (SkinRendererList.Length > 0)
        {
            System.Random random = new System.Random();
            Renderer = SkinRendererList[random.Next(SkinRendererList.Length)];
        }
        
        Renderer.gameObject.SetActive(true);
        
        _healthSystem.SetRender(Renderer);
        _enemyDeathEffect.SetRenderer(Renderer);

        _baseColor = Renderer.material.color;
    }

    private void OnEnable()
    {
        _healthSystem.enabled = true;
        _targetSystem.enabled = true;
        _attackController.enabled = true;
        _enemyController.enabled = true;
        _enemyDeathEffect.enabled = true;
    }

    private void OnDisable()
    {
        Renderer.material.color = _baseColor;
        
        _healthSystem.enabled = false;
        _targetSystem.enabled = false;
        _attackController.enabled = false;
        _enemyController.enabled = false;
        _enemyDeathEffect.enabled = false;
    }

    public Enemy Setup(Vector3 position, Quaternion rotation, Wave wave = null)
    {
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = Size;

        if (wave != null)
        {
            Health += wave.waveStruct.health;
            Damage += wave.waveStruct.damage;
            MoveSpeed += wave.waveStruct.moveSpeed;
            XpOnDeath += wave.waveStruct.xpOnDeath;
        }

        _healthSystem.Init(Health);

        return this;
    }

    public void Die()
    {
        Renderer.material.color = _baseColor;
        EnemyPool.Instance.ReturnToPool(this);
    }

    public TargetSystem GetEnemyTargetSystem => _targetSystem;
    public HealthSystem GetEnemyHealthSystem => _healthSystem;
    public EnemyAttackController GetEnemyAttackController => _attackController;
    public EnemyController GetEnemyController => _enemyController;

    public bool IsPower => _isPower;
}

public enum ZombieType
{
    STANDARD,
    FAST, 
    FAT
}
