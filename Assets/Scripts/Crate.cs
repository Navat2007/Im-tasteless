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
    [SerializeField] private float _spawnHeight = 0.5f;

    [Header("Дробовик")]
    [SerializeField] private float _shotgunSpawnChance = 40;
    
    [Header("Автомат")]
    [SerializeField] private float _rifleSpawnChance = 40;
    
    [Header("Гранаты")]
    [SerializeField] private float _grenadeSpawnChance = 20;

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

    private void OnDeath(GameObject owner, ProjectileHitInfo projectileHitInfo)
    {
        WeaponType GetWeaponToSpawn()
        {
            float[] percentages = { _shotgunSpawnChance / 100, _rifleSpawnChance / 100, _grenadeSpawnChance / 100 };

            float totalPercentage = 0f;
            float randomValue = UnityEngine.Random.value;

            WeaponType selectedValue = WeaponType.SHOTGUN;

            for (int i = 1; i < percentages.Length; i++)
            {
                totalPercentage += percentages[i];

                if (randomValue <= totalPercentage)
                {
                    selectedValue = (WeaponType)i;
                    break;
                }
            }
        
            return selectedValue;
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
                .Setup(new Vector3(transform.position.x, _spawnHeight, transform.position.z), Quaternion.identity)
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
