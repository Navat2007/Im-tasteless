using Interface;
using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class Enemy : MonoBehaviour, IHealth, IDamageable
{
    [field: SerializeField] public ZombieType ZombieType { get; private set; }
    [field: SerializeField] public float Health { get; set; }
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float AttackDistance { get; private set; }
    [field: SerializeField] public float MsBetweenAttack { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public int XpOnDeath { get; private set; } = 1;

    public void SetHealth(float value)
    {
        Health = value;
    }
    
    public void SetDamage(float value)
    {
        Damage = value;
    }
    
    public void SetAttackDistance(float value)
    {
        AttackDistance = value;
    }
    
    public void SetMsBetweenAttack(float value)
    {
        MsBetweenAttack = value;
    }
    
    public void SetMoveSpeed(float value)
    {
        MoveSpeed = value;
    }

    public void SetSize(Vector3 size)
    {
        transform.localScale = size;
    }

    public Color GetColor()
    {
        return GetComponent<MeshRenderer>().material.color;
    }

    public void SetColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
    }
    
    public void SetXpOnDeath(int amount)
    {
        XpOnDeath = amount;
    }

    public void SetZombieType(ZombieType type)
    {
        ZombieType = type;
    }
}

public enum ZombieType
{
    STANDARD,
    FAST, 
    STRONG
}
