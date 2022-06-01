using UnityEngine;

public class Weapon : MonoBehaviour
{
    [field: SerializeField] public WeaponType CurrentWeaponType { get; private set; }
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float PeriodDamage { get; private set; }
    [field: SerializeField] public float PeriodDamageTick { get; private set; }
    [field: SerializeField] public float PeriodDamageDuration { get; private set; }
    [field: SerializeField] public float CriticalChance { get; private set; } = 10;
    [field: SerializeField] public float CriticalBonus { get; private set; } = 2;
    [field: SerializeField] public float ReloadSpeed { get; private set; }
    [field: SerializeField] public float MsBetweenShots { get; private set; } = 100;
    [field: SerializeField] public float RecoilPower { get; private set; } = .2f;
    
    [field: Header("Audio")]
    [field: SerializeField] public AudioClip ShootClip { get; private set; }
    [field: SerializeField] public AudioClip EmptyAmmoClip { get; private set; }
    
    [field: Header("Projectile")]
    [field: SerializeField] public bool InfiniteProjectile { get; private set; }
    [field: SerializeField] public bool KnockBack { get; private set; }
    [field: SerializeField] public ProjectileType ProjectileType { get; private set; }
    [field: SerializeField] public Transform[] ProjectileSpawnPoint { get; private set; }
    [field: SerializeField] public int ShootPointCount { get; private set; }
    [field: SerializeField] public int ProjectilePerClip { get; private set; }
    [field: SerializeField] public int ProjectileInClip { get; set; }
    [field: SerializeField] public int CurrentProjectileAmount { get; set; }
    [field: SerializeField] public int MaxProjectileAmount { get; set; }
    [field: SerializeField] public int PenetrateCount { get; private set; }
    [field: SerializeField] public float KillChance { get; private set; }
    
    [field: Header("Shell")]
    [field: SerializeField] public ShellType ShellType { get; private set; }
    [field: SerializeField] public Transform ShellSpawnPoint { get; private set; }

    [field: Header("Muzzle")]
    [field: SerializeField] public ParticleSystem MuzzleParticleSystem { get; private set; }
    [field: SerializeField] public Transform MuzzlePoint { get; private set; }
    [field: SerializeField] public float MuzzleVelocity { get; private set; } = 35;

    public void AddDamage(float value)
    {
        Damage += value;
    }
    
    public void SetDamage(float value)
    {
        Damage = value;
    }
    
    public void AddPeriodDamage(float value)
    {
        PeriodDamage += value;
    }
    
    public void SetPeriodDamage(float value)
    {
        PeriodDamage = value;
    }
    
    public void AddPeriodDamageTick(float value)
    {
        PeriodDamageTick += value;
    }
    
    public void SetPeriodDamageTick(float value)
    {
        PeriodDamageTick = value;
    }
    
    public void AddPeriodDamageDuration(float value)
    {
        PeriodDamageDuration += value;
    }
    
    public void SetPeriodDamageDuration(float value)
    {
        PeriodDamageDuration = value;
    }
    
    public void SetReloadSpeed(float value)
    {
        ReloadSpeed = value;
    }
    
    public void SetAttackSpeed(float value)
    {
        MsBetweenShots = value;
    }
    
    public void SetCriticalChance(float value)
    {
        CriticalChance = value;
    }
    
    public void SetCriticalBonus(float value)
    {
        CriticalBonus = value;
    }
    
    public void SetProjectilePerClip(int value)
    {
        ProjectilePerClip = value;
    }
    
    public void SetAudioClip(AudioClip clip)
    {
        ShootClip = clip;
    }

    public void AddPenetrateCount(int value)
    {
        PenetrateCount += value;
    }
    
    public void SetPenetrateCount(int value)
    {
        PenetrateCount = value;
    }
    
    public void AddKillChance(int value)
    {
        KillChance += value;
    }
    
    public void SetKillChance(int value)
    {
        KillChance = value;
    }

    public void ActivateMoreProjectilePoints(int count)
    {
        int needToActivate = count;
        foreach (var point in ProjectileSpawnPoint)
        {
            if (point.gameObject.activeSelf) continue;
            
            point.gameObject.SetActive(true);
            needToActivate--;
                
            if(needToActivate <= 0)
                break;
        }
    }

    public void SetKnockBack(bool value)
    {
        KnockBack = value;
    }
}

public enum WeaponType
{
    PISTOL,
    SHOTGUN,
    RIFLE,
    GRENADE
}

public enum FireMod
{
    SINGLE,
    BURST,
    AUTO
}

