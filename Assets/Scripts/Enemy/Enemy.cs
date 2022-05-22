using System;
using Interface;
using UnityEngine;

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
    
    [field: Header("Усиленные параметры")]
    [field: SerializeField] public bool IsPower { get; set; }
    [field: SerializeField] public float PowerHealth { get; private set; }
    [field: SerializeField] public float PowerDamage { get; private set; }
    [field: SerializeField] public float PowerCriticalChance { get; private set; }
    [field: SerializeField] public float PowerCriticalBonus { get; private set; }
    [field: SerializeField] public float PowerAttackDistance { get; private set; }
    [field: SerializeField] public float PowerMsBetweenAttack { get; private set; }
    [field: SerializeField] public float PowerMoveSpeed { get; private set; }
    [field: SerializeField] public float PowerTurnSpeed { get; private set; }
    [field: SerializeField] public int PowerXpOnDeath { get; private set; }
    [field: SerializeField] public Color PowerColor { get; private set; }
    [field: SerializeField] public Vector3 PowerSize { get; private set; }

    private TargetSystem _targetSystem;
    private HealthSystem _healthSystem;
    private EnemyAttackController _attackController;
    private EnemyController _enemyController;
    private EnemyDeathEffect _enemyDeathEffect;

    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();
        _targetSystem = GetComponent<TargetSystem>();
        _attackController = GetComponent<EnemyAttackController>();
        _enemyController = GetComponent<EnemyController>();
        _enemyDeathEffect = GetComponent<EnemyDeathEffect>();

        if (SkinRendererList.Length > 0)
        {
            System.Random random = new System.Random();
            Renderer = SkinRendererList[random.Next(SkinRendererList.Length)];
        }
        
        Renderer.gameObject.SetActive(true);
    }

    private void Start()
    {
        Setup();
        
        _healthSystem.SetRender(Renderer);
        _enemyDeathEffect.SerRenderer(Renderer);
    }

    public void Setup(Wave wave = null)
    {
        transform.localScale = Size;

        if (IsPower)
        {
            Health = PowerHealth;
            Damage = PowerDamage;
            CriticalChance = PowerCriticalChance;
            CriticalBonus = PowerCriticalBonus;
            AttackDistance = PowerAttackDistance;
            MsBetweenAttack = PowerMsBetweenAttack;
            MoveSpeed = PowerMoveSpeed;
            TurnSpeed = PowerTurnSpeed;
            XpOnDeath = PowerXpOnDeath;
            
            transform.localScale = PowerSize;
            
            gameObject
                .AddComponent<EnemyPowerSkin>()
                .SetZombieType(ZombieType)
                .SetRenderer(Renderer)
                .SetColor(Renderer.material.color);

            switch (ZombieType)
            {
                case ZombieType.FAT:
                    gameObject.AddComponent<EnemyFatController>();
                    break;
                case ZombieType.FAST:
                    gameObject.AddComponent<EnemyFastController>();
                    break;
                case ZombieType.STANDARD:
                    gameObject.AddComponent<EnemyStandardController>();
                    break;
            }
        }

        if (wave != null)
        {
            Health += wave.waveStruct.health;
            Damage += wave.waveStruct.damage;
            MoveSpeed += wave.waveStruct.moveSpeed;
            XpOnDeath += wave.waveStruct.xpOnDeath;
        }

        _healthSystem.Init(Health);
    }

    public TargetSystem GetEnemyTargetSystem => _targetSystem;
    public HealthSystem GetEnemyHealthSystem => _healthSystem;
    public EnemyAttackController GetEnemyAttackController => _attackController;
    public EnemyController GetEnemyController => _enemyController;

    [ContextMenu("Сделать усиленным")]
    private void MakePower()
    {
        IsPower = true;
        Setup();
    }
}

public enum ZombieType
{
    STANDARD,
    FAST, 
    FAT
}
