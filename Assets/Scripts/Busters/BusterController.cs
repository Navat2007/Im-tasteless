using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WeaponController))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(HealthSystem))]
public class BusterController : MonoBehaviour
{
    private WeaponController _weaponController;
    private PlayerController _playerController;
    private HealthSystem _healthSystem;

    private bool _isMoveSpeedBusterActive;
    private float _moveSpeedTimer;

    private void Awake()
    {
        _weaponController = GetComponent<WeaponController>();
        _playerController = GetComponent<PlayerController>();
        _healthSystem = GetComponent<HealthSystem>();
    }

    public void PickFirstAidKit(int count, float percent)
    {
        float healthToAdd = _healthSystem.MaxHealth / 100 * percent;
        _healthSystem.AddHealth(healthToAdd);
    }
    
    public void PickBandage(int count, float percent, float tickTimePeriod, int tickAmount)
    {
        float healthToAdd = _healthSystem.MaxHealth / 100 * percent;
        _healthSystem.AddHealth(healthToAdd, tickTimePeriod, tickAmount);
    }
    
    public void PickClip(int count)
    {
        foreach (WeaponType weapon in Enum.GetValues(typeof(WeaponType)))
        {
            switch (weapon)
            {
                case WeaponType.SHOTGUN:
                    if(_weaponController.IsWeaponActive(WeaponType.SHOTGUN))
                        _weaponController.AddAmmo(_weaponController.GetWeaponInfo(WeaponType.SHOTGUN).ammoPerClip, WeaponType.SHOTGUN, false);
                    break;
                case WeaponType.RIFLE:
                    if(_weaponController.IsWeaponActive(WeaponType.RIFLE))
                        _weaponController.AddAmmo(_weaponController.GetWeaponInfo(WeaponType.RIFLE).ammoPerClip, WeaponType.RIFLE, false);
                    break;
            }
        }
    }
    
    public void PickGrenade(int count)
    {
        _weaponController.AddAmmo(count, WeaponType.GRENADE, true);
    }
    
    public void PickBodyArmor(int count)
    {
        _healthSystem.AddArmor(count);
    }
    
    public void PickDamage(int count)
    {
        IEnumerator Activate()
        {
            
            
            yield return null;
        }
    }
    
    public void PickAttackSpeed(int count)
    {
        
    }
    
    public void PickMoveSpeed(int count, float duration)
    {
        IEnumerator Activate()
        {
            _isMoveSpeedBusterActive = true;
            _moveSpeedTimer = Time.time + duration;
            
            
            yield return null;
            
            _isMoveSpeedBusterActive = false;
        }
    }
}
