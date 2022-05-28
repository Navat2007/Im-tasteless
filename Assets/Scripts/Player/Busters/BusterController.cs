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
    
    [Header("Pools")]
    [SerializeField] private Transform busterPool;

    [Header("Prefabs")]
    [SerializeField] private GameObject firstAidKitPrefab;
    [SerializeField] private GameObject bandagePrefab;
    [SerializeField] private GameObject clipPrefab;
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private GameObject armorPrefab;
    [SerializeField] private GameObject damagePrefab;
    [SerializeField] private GameObject attackSpeedPrefab;
    [SerializeField] private GameObject moveSpeedPrefab;
    
    private bool _isMoveSpeedBusterActive;
    private float _moveSpeedTimer;
    
    private bool _isDamageBusterActive;
    private float _damageTimer;
    
    private bool _isAttackSpeedBusterActive;
    private float _attackSpeedTimer;
    
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
        float healthToAdd = ControllerManager.healthSystem.MaxHealth / 100 * percent;
        ControllerManager.healthSystem.AddHealth(healthToAdd);
    }
    
    public void PickBandage(int count, float percent, float tickTimePeriod, int tickAmount)
    {
        float healthToAdd = ControllerManager.healthSystem.MaxHealth / 100 * percent;
        ControllerManager.healthSystem.AddHealth(healthToAdd, tickTimePeriod, tickAmount);
    }
    
    public void PickClip(int count)
    {
        foreach (WeaponType weapon in Enum.GetValues(typeof(WeaponType)))
        {
            switch (weapon)
            {
                case WeaponType.SHOTGUN:
                    if(ControllerManager.weaponController.IsWeaponActive(WeaponType.SHOTGUN))
                    {
                        var ammoPerClip = ControllerManager.weaponController.GetWeaponInfo(WeaponType.SHOTGUN).GetAmmoPerClip();
                        ControllerManager.weaponController.AddAmmo(ammoPerClip, WeaponType.SHOTGUN, false);
                    }    
                    break;
                case WeaponType.RIFLE:
                    if (ControllerManager.weaponController.IsWeaponActive(WeaponType.RIFLE))
                    {
                        var ammoPerClip = ControllerManager.weaponController.GetWeaponInfo(WeaponType.RIFLE).GetAmmoPerClip();
                        ControllerManager.weaponController.AddAmmo(ammoPerClip, WeaponType.RIFLE, false);
                    }
                    break;
            }
        }
    }
    
    public void PickGrenade(int count)
    {
        ControllerManager.weaponController.AddAmmo(count, WeaponType.GRENADE, true);
    }
    
    public void PickBodyArmor(int count)
    {
        ControllerManager.healthSystem.AddArmor(count);
    }
    
    public void PickDamage(int count, float duration, float damagePercent)
    {
        IEnumerator Activate()
        {
            _isDamageBusterActive = true;
            ControllerManager.weaponController.AddBonusDamagePercent(damagePercent);

            while (_damageTimer > 0)
            {
                _damageTimer -= Time.deltaTime;
                OnDamageChange?.Invoke(_damageTimer);
                yield return null;
            }

            _isDamageBusterActive = false;
            ControllerManager.weaponController.AddBonusDamagePercent(-damagePercent);
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
            ControllerManager.weaponController.AddBonusAttackSpeedPercent(speedPercent);
            ControllerManager.weaponController.AddBonusReloadSpeedPercent(speedPercent);

            while (_attackSpeedTimer > 0)
            {
                _attackSpeedTimer -= Time.deltaTime;
                OnAttackSpeedChange?.Invoke(_attackSpeedTimer);
                yield return null;
            }

            _isAttackSpeedBusterActive = false;
            ControllerManager.weaponController.AddBonusAttackSpeedPercent(-speedPercent);
            ControllerManager.weaponController.AddBonusReloadSpeedPercent(-speedPercent);
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
            float speedToAdd = ControllerManager.player.MoveSpeed / 100 * moveSpeedPercent;
            ControllerManager.player.AddBonusMoveSpeed(speedToAdd);

            while (_moveSpeedTimer > 0)
            {
                _moveSpeedTimer -= Time.deltaTime;
                OnMoveSpeedChange?.Invoke(_moveSpeedTimer);
                yield return null;
            }

            _isMoveSpeedBusterActive = false;
            ControllerManager.player.AddBonusMoveSpeed(-speedToAdd);
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

    public void SpawnBuster(Vector3 position)
    {
        Array values = Enum.GetValues(typeof(BusterType));
        System.Random random = new System.Random();
        BusterType busterType = (BusterType)values.GetValue(random.Next(values.Length));

        var buster = BusterPool.Instance.Get(busterType);
        buster.Setup(new Vector3(position.x, 1, position.z), Quaternion.identity);
        buster.gameObject.SetActive(true);
    }
}
