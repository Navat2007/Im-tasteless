using System.Collections.Generic;
using UnityEngine;

public class PickableWeaponPool : MonoBehaviour
{
    public static PickableWeaponPool Instance { get; private set; }

    [Header("Prefabs")] 
    [SerializeField] private PickableWeapon grenadePickableWeaponPrefab;
    [SerializeField] private PickableWeapon shotgunPickableWeaponPrefab;
    [SerializeField] private PickableWeapon riflePickableWeaponPrefab;

    [Header("Pools")] 
    [SerializeField] private Transform grenadePickableWeaponPool;
    [SerializeField] private Transform shotgunPickableWeaponPool;
    [SerializeField] private Transform riflePickableWeaponPool;

    private Queue<PickableWeapon> _grenadePickableWeapons = new();
    private Queue<PickableWeapon> _shotgunPickableWeapons = new();
    private Queue<PickableWeapon> _riflePickableWeapons = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GeneratePools();
    }

    private void GeneratePools()
    {
        AddPickableWeapon(20, WeaponType.GRENADE);
        AddPickableWeapon(20, WeaponType.SHOTGUN);
        AddPickableWeapon(20, WeaponType.RIFLE);
    }

    private void AddPickableWeapon(int count, WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.GRENADE:
                for (int i = 0; i < count; i++)
                {
                    var pickableWeapon = Instantiate(grenadePickableWeaponPrefab, grenadePickableWeaponPool);
                    pickableWeapon.SetWeaponType(weaponType);
                    pickableWeapon.gameObject.SetActive(false);
                    _grenadePickableWeapons.Enqueue(pickableWeapon);
                }
                break;
            case WeaponType.SHOTGUN:
                for (int i = 0; i < count; i++)
                {
                    var pickableWeapon = Instantiate(shotgunPickableWeaponPrefab, shotgunPickableWeaponPool);
                    pickableWeapon.SetWeaponType(weaponType);
                    pickableWeapon.gameObject.SetActive(false);
                    _shotgunPickableWeapons.Enqueue(pickableWeapon);
                }
                break;
            case WeaponType.RIFLE:
                for (int i = 0; i < count; i++)
                {
                    var pickableWeapon = Instantiate(riflePickableWeaponPrefab, riflePickableWeaponPool);
                    pickableWeapon.SetWeaponType(weaponType);
                    pickableWeapon.gameObject.SetActive(false);
                    _riflePickableWeapons.Enqueue(pickableWeapon);
                }
                break;
        }
    }

    public PickableWeapon Get(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.GRENADE:
                if (_grenadePickableWeapons.Count == 0) AddPickableWeapon(1, weaponType);
                return _grenadePickableWeapons.Dequeue();
            case WeaponType.SHOTGUN:
                if (_shotgunPickableWeapons.Count == 0) AddPickableWeapon(1, weaponType);
                return _shotgunPickableWeapons.Dequeue();
            case WeaponType.RIFLE:
                if (_riflePickableWeapons.Count == 0) AddPickableWeapon(1, weaponType);
                return _riflePickableWeapons.Dequeue();
        }

        return null;
    }

    public void ReturnToPool(PickableWeapon pickableWeapon)
    {
        pickableWeapon.gameObject.SetActive(false);

        switch (pickableWeapon.GetWeaponType)
        {
            case WeaponType.GRENADE:
                _grenadePickableWeapons.Enqueue(pickableWeapon);
                break;
            case WeaponType.SHOTGUN:
                _shotgunPickableWeapons.Enqueue(pickableWeapon);
                break;
            case WeaponType.RIFLE:
                _riflePickableWeapons.Enqueue(pickableWeapon);
                break;
        }
    }
}