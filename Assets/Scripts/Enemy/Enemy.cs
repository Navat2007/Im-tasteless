using System;
using Interface;
using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class Enemy : MonoBehaviour, IHealth, IDamageable
{
    [field: SerializeField] public ZombieType ZombieType { get; private set; }
    
    [field: Header("Стандартные параметры")]
    [field: SerializeField] public float Health { get; set; }
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float AttackDistance { get; private set; }
    [field: SerializeField] public float MsBetweenAttack { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public int XpOnDeath { get; private set; } = 1;
    [field: SerializeField] public Color Color { get; private set; }
    [field: SerializeField] public Vector3 Size { get; private set; }
    
    [field: Header("Усиленные параметры")]
    [field: SerializeField] public bool IsPower { get; set; }
    [field: SerializeField] public float PowerHealth { get; private set; }
    [field: SerializeField] public float PowerDamage { get; private set; }
    [field: SerializeField] public float PowerAttackDistance { get; private set; }
    [field: SerializeField] public float PowerMsBetweenAttack { get; private set; }
    [field: SerializeField] public float PowerMoveSpeed { get; private set; }
    [field: SerializeField] public int PowerXpOnDeath { get; private set; }
    [field: SerializeField] public Color PowerColor { get; private set; }
    [field: SerializeField] public Vector3 PowerSize { get; private set; }

    private void Start()
    {
        Setup();
    }

    public void Setup(WaveSO waveSo = null)
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        
        transform.localScale = Size;
        meshRenderer.material.color = Color;

        if (IsPower)
        {
            Health = PowerHealth;
            Damage = PowerDamage;
            AttackDistance = PowerAttackDistance;
            MsBetweenAttack = PowerMsBetweenAttack;
            MoveSpeed = PowerMoveSpeed;
            XpOnDeath = PowerXpOnDeath;
            
            transform.localScale = PowerSize;
            meshRenderer.material.color = PowerColor;
            
            gameObject
                .AddComponent<EnemyPowerSkin>()
                .SetColor(meshRenderer.material.color);

            //TODO поведение усиленного зомби
            switch (ZombieType)
            {
                case ZombieType.FAT:
                    break;
                case ZombieType.FAST:
                    break;
                case ZombieType.STANDARD:
                    break;
            }
        }

        if (waveSo != null)
        {
            Health += waveSo.health;
            Damage += waveSo.damage;
            MoveSpeed += waveSo.moveSpeed;
            XpOnDeath += waveSo.xpOnDeath;
        }
    }
}

public enum ZombieType
{
    STANDARD,
    FAST, 
    FAT
}
