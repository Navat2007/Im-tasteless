using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WeaponController))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(HealthSystem))]
public class BusterController : MonoBehaviour
{
    public event Action<float> OnMoveSpeedChange;
    public event Action<float> OnDamageChange;
    public event Action<float> OnAttackSpeedChange;

    [SerializeField] private GameObject firstAidKitPrefab;
    [SerializeField] private GameObject bandagePrefab;
    [SerializeField] private GameObject clipPrefab;
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private GameObject armorPrefab;
    [SerializeField] private GameObject damagePrefab;
    [SerializeField] private GameObject attackSpeedPrefab;
    [SerializeField] private GameObject moveSpeedPrefab;
    
    private Player _player;
    private WeaponController _weaponController;
    private PlayerController _playerController;
    private HealthSystem _healthSystem;

    private bool _isMoveSpeedBusterActive;
    private float _moveSpeedTimer;
    
    private bool _isDamageBusterActive;
    private float _damageTimer;
    
    private bool _isAttackSpeedBusterActive;
    private float _attackSpeedTimer;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _weaponController = GetComponent<WeaponController>();
        _playerController = GetComponent<PlayerController>();
        _healthSystem = GetComponent<HealthSystem>();
    }
    
    private void OnEnable()
    {
        ControllerManager.busterController = this;
    }

    private void OnDisable()
    {
        ControllerManager.busterController = null;
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
                    {
                        var ammoPerClip = _weaponController.GetWeaponInfo(WeaponType.SHOTGUN).GetAmmoPerClip();
                        _weaponController.AddAmmo(ammoPerClip, WeaponType.SHOTGUN, false);
                    }    
                    break;
                case WeaponType.RIFLE:
                    if (_weaponController.IsWeaponActive(WeaponType.RIFLE))
                    {
                        var ammoPerClip = _weaponController.GetWeaponInfo(WeaponType.RIFLE).GetAmmoPerClip();
                        _weaponController.AddAmmo(ammoPerClip, WeaponType.RIFLE, false);
                    }
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
    
    public void PickDamage(int count, float duration, float damagePercent)
    {
        IEnumerator Activate()
        {
            _isDamageBusterActive = true;
            _weaponController.AddBonusDamagePercent(damagePercent);

            while (_damageTimer > 0)
            {
                _damageTimer -= Time.deltaTime;
                OnDamageChange?.Invoke(_damageTimer);
                yield return null;
            }

            _isDamageBusterActive = false;
            _weaponController.AddBonusDamagePercent(-damagePercent);
        }
        
        _damageTimer = duration;
        
        if(!_isDamageBusterActive)
            StartCoroutine(Activate());
    }
    
    public void PickAttackSpeed(int count, float duration, float speedPercent)
    {
        IEnumerator Activate()
        {
            _isAttackSpeedBusterActive = true;
            _weaponController.AddBonusAttackSpeedPercent(speedPercent);
            _weaponController.AddBonusReloadSpeedPercent(speedPercent);

            while (_attackSpeedTimer > 0)
            {
                _attackSpeedTimer -= Time.deltaTime;
                OnAttackSpeedChange?.Invoke(_attackSpeedTimer);
                yield return null;
            }

            _isAttackSpeedBusterActive = false;
            _weaponController.AddBonusAttackSpeedPercent(-speedPercent);
            _weaponController.AddBonusReloadSpeedPercent(-speedPercent);
        }
        
        _attackSpeedTimer = duration;
        
        if(!_isAttackSpeedBusterActive)
            StartCoroutine(Activate());
    }
    
    public void PickMoveSpeed(int count, float duration, float moveSpeedPercent)
    {
        IEnumerator Activate()
        {
            _isMoveSpeedBusterActive = true;
            float speedToAdd = _player.MoveSpeed / 100 * moveSpeedPercent;
            _player.AddBonusMoveSpeed(speedToAdd);

            while (_moveSpeedTimer > 0)
            {
                _moveSpeedTimer -= Time.deltaTime;
                OnMoveSpeedChange?.Invoke(_moveSpeedTimer);
                yield return null;
            }

            _isMoveSpeedBusterActive = false;
            _player.AddBonusMoveSpeed(-speedToAdd);
        }
        
        _moveSpeedTimer = duration;
        
        if(!_isMoveSpeedBusterActive)
            StartCoroutine(Activate());
    }

    public GameObject GetBusterPrefab(BusterType busterType)
    {
        switch (busterType)
        {
            case BusterType.FIRST_AID_KIT:
                return firstAidKitPrefab;
            case BusterType.BANDAGE:
                return bandagePrefab;
            case BusterType.CLIP:
                return clipPrefab;
            case BusterType.GRENADE:
                return grenadePrefab;
            case BusterType.BODY_ARMOR:
                return armorPrefab;
            case BusterType.DAMAGE:
                return damagePrefab;
            case BusterType.ATTACK_SPEED:
                return attackSpeedPrefab;
            case BusterType.MOVE_SPEED:
                return moveSpeedPrefab;
            default:
                return null;
        }
    }
}
