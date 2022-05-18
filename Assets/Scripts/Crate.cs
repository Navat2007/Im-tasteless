using System;
using Interface;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthSystem))]
public class Crate : MonoBehaviour, IDamageable, IHealth
{
    [field: SerializeField] public float Health { get; set; }
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

    private HealthSystem _healthSystem;
    private bool _spawned;

    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();
        
        _healthSystem.OnDeath += OnDeath;
    }

    private void OnDeath(ProjectileHitInfo projectileHitInfo)
    {
        GameObject GetWeaponToSpawn()
        {
            int count = 0;

            while (count < 1000)
            {
                Array values = Enum.GetValues(typeof(WeaponType));
                WeaponType weaponType = (WeaponType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
            
                switch (weaponType)
                {
                    case WeaponType.SHOTGUN:
                        if (Helper.GetCriticalChance(shotgunSpawnChance))
                        {
                            return shotgunPrefab;
                        }
                        break;
                    case WeaponType.RIFLE:
                        if (Helper.GetCriticalChance(rifleSpawnChance))
                        {
                            return riflePrefab;
                        }
                        break;
                    case WeaponType.GRENADE:
                        if (Helper.GetCriticalChance(grenadeSpawnChance))
                        {
                            return grenadePrefab;
                        }
                        break;
                }

                count++;
            }
        
            //print($"Оружие заспавнено с {count} попытки");
        
            return grenadePrefab;
        }
        
        if(_spawned) return;

        _spawned = true;
        
        Instantiate(
            GetWeaponToSpawn(), 
            new Vector3(transform.position.x, spawnHeight, transform.position.z), 
            Quaternion.identity, 
            GameObject.FindGameObjectWithTag("WeaponPool").transform
        );
        
        Destroy(gameObject);
    }
}
