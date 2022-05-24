using System;
using System.Collections.Generic;
using Interface;
using Pools;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthSystem))]
public class Crate : MonoBehaviour, IDamageable, IHealth
{
    [field: SerializeField] public float Health { get; set; }
    [field: SerializeField] public Renderer Renderer { get; private set; }
    [SerializeField] private float spawnHeight = 0.5f;

    [Header("Дробовик")]
    [SerializeField] private GameObject shotgunPrefab;
    [SerializeField] private float shotgunSpawnChance = 40;
    
    [Header("Автомат")]
    [SerializeField] private GameObject riflePrefab;
    [SerializeField] private float rifleSpawnChance = 40;
    
    [Header("Гранаты")]
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private float grenadeSpawnChance = 20;
    [SerializeField] private float grenadePowerCrateCount = 10;

    private HealthSystem _healthSystem;
    private bool _spawned;
    private SpawnPoint _spawnPoint;

    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();
        
        _healthSystem.SetRender(Renderer);
        _healthSystem.Init(Health);
        _healthSystem.OnDeath += OnDeath;
    }

    private void OnDeath(ProjectileHitInfo projectileHitInfo)
    {
        WeaponType GetWeaponToSpawn()
        {
            int count = 0;

            while (count < 1000)
            {
                Array values = Enum.GetValues(typeof(WeaponType));
                System.Random random = new System.Random();
                WeaponType weaponType = (WeaponType)values.GetValue(random.Next(values.Length));
            
                switch (weaponType)
                {
                    case WeaponType.SHOTGUN:
                        if (Helper.IsCritical(shotgunSpawnChance))
                        {
                            return weaponType;
                        }
                        break;
                    case WeaponType.RIFLE:
                        if (Helper.IsCritical(rifleSpawnChance))
                        {
                            return weaponType;
                        }
                        break;
                    case WeaponType.GRENADE:
                        if (Helper.IsCritical(grenadeSpawnChance))
                        {
                            return weaponType;
                        }
                        break;
                }

                count++;
            }
        
            return WeaponType.GRENADE;
        }

        void AddWeaponAmmo()
        {
            List<WeaponType> activeWeapon = new() { WeaponType.GRENADE };

            var shotgun = ControllerManager.weaponController.GetWeaponInfo(WeaponType.SHOTGUN);
            var rifle = ControllerManager.weaponController.GetWeaponInfo(WeaponType.RIFLE);

            if (shotgun.isActive && shotgun.ammoPerClip < shotgun.ammoMax) activeWeapon.Add(WeaponType.SHOTGUN);
            if (rifle.isActive && rifle.ammoPerClip < rifle.ammoMax) activeWeapon.Add(WeaponType.RIFLE);

            System.Random random = new System.Random();
            var weapon = activeWeapon[random.Next(activeWeapon.Count)];
            ControllerManager.weaponController.AddAmmo(ControllerManager.weaponController.GetWeaponInfo(weapon).ammoMax, weapon, false);
            //TODO показать всплывающий значок
        }
        
        if(_spawned) return;

        _spawned = true;

        if (ControllerManager.crateSpawner.IsNextPowerCrate)
        {
            AddWeaponAmmo();
            
            if (ControllerManager.crateSpawner.IsDoublePowerCrate)
                AddWeaponAmmo();
            
            ControllerManager.crateSpawner.SetPowerCrate(false, false);
        }
        else
        {
            var pickableWeapon = PickableWeaponPool.Instance.Get(GetWeaponToSpawn());
            pickableWeapon
                .Setup(new Vector3(transform.position.x, spawnHeight, transform.position.z), Quaternion.identity)
                .gameObject.SetActive(true);
        }
        
        ControllerManager.crateSpawner.RemoveSpawnPoint(_spawnPoint);

        CratePool.Instance.ReturnToPool(this);
    }

    public Crate SetSpawnPoint(SpawnPoint spawnPoint)
    {
        _spawnPoint = spawnPoint;
        
        return this;
    }

    public Crate Setup(Vector3 position, Quaternion rotation)
    {
        _spawned = false;
        transform.position = position;
        transform.rotation = rotation;
        
        return this;
    }
}
