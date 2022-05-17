using UnityEngine;

public class Weapon : MonoBehaviour
{
    [field: SerializeField] public WeaponType CurrentWeaponType { get; private set; }
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float CriticalChance { get; private set; } = 10;
    [field: SerializeField] public float CriticalBonus { get; private set; } = 2;
    [field: SerializeField] public float ReloadSpeed { get; private set; }
    [field: SerializeField] public float MsBetweenShots { get; private set; } = 100;
    [field: SerializeField] public float RecoilPower { get; private set; } = .2f;
    
    [field: Header("Audio")]
    [field: SerializeField] public AudioClip ShootClip { get; private set; }
    
    [field: Header("Projectile")]
    [field: SerializeField] public bool infiniteProjectile { get; private set; }
    [field: SerializeField] public Projectile Projectile { get; private set; }
    [field: SerializeField] public Transform[] ProjectileSpawnPoint { get; private set; }
    [field: SerializeField] public int ProjectilePerClip { get; private set; }
    [field: SerializeField] public int ProjectileInClip { get; set; }
    [field: SerializeField] public int CurrentProjectileAmount { get; set; }
    [field: SerializeField] public int MaxProjectileAmount { get; set; }
    
    [field: Header("Shell")]
    [field: SerializeField] public Shell Shell { get; private set; }
    [field: SerializeField] public Transform ShellSpawnPoint { get; private set; }

    [field: Header("Muzzle")]
    [field: SerializeField] public ParticleSystem MuzzleParticleSystem { get; private set; }
    [field: SerializeField] public Transform MuzzlePoint { get; private set; }
    [field: SerializeField] public float MuzzleVelocity { get; private set; } = 35;
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
