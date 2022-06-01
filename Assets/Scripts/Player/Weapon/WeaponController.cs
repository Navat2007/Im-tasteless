using System;
using System.Collections;
using System.Collections.Generic;
using Pools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Animator))]
public class WeaponController : MonoBehaviour
{
    public event Action OnReloadStart; 
    public event Action OnReloadEnd; 
    public event Action<float> OnReloadPercent; 
    public event Action<int, int, bool, WeaponType> OnAmmoChange;
    public event Action<WeaponType> OnEquipWeapon;

    public Transform GetWeaponHold => weaponHold;

    [SerializeField] private WeaponType startingWeapon;
    
    [Header("Hold")]
    [SerializeField] private Transform weaponHold;
    [SerializeField] private Transform leftHandHold;

    [Header("Weapons List")] 
    [SerializeField] private Weapon pistol;
    [SerializeField] private Weapon shotgun;
    [SerializeField] private Weapon rifle;
    
    [Header("Grenade")] 
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private bool infiniteGrenade;
    [SerializeField] private bool doubleGrenade;
    [SerializeField] private int grenadeCount;
    [SerializeField] private int grenadeMaxCount = 10;
    [SerializeField] private float grenadeCooldownMs = 800;
    [SerializeField] private float grenadeMaxThrowingForce = 15;
    [SerializeField] private float grenadeThrowingUpwardForce = 4;

    private GameUI _gameUI;
    private PlayerInput _playerInput;
    private PlayerController _playerController;
    private Animator _animator;
    private AudioManager _audioManager;
    private Weapon _equippedWeapon;

    private const bool _isPistolActive = true;
    private bool _isShogunActive;
    private bool _isRifleActive;
    private bool _isGrenadeActive;

    private float _nextShootTime;
    private float _nextThrowingTime;
    private float _nextEmptyShootTime;
    private Vector3 _recoilSmoothDampVelocity;
    private float _recoilRotationSmoothDampVelocity;
    private float _recoilAngle;
    private Vector3 _initialRotation;
    private Vector3 _crosshairPosition;

    private bool _isReloading;
    private float _bonusDamagePercent;
    private float _bonusAttackSpeedPercent;
    private float _bonusReloadSpeedPercent;
    private float _bonusCriticalChance;
    private float _bonusCriticalBonus;
    private int _bonusMaxClip;
    private int _bonusTakeClip;
    private int _bonusPenetrateCount;
    private float _bonusKillChance;

    private bool _halfMaxClip;
    private bool _doubleMaxClip;
    
    public Weapon GetEquippedWeapon => _equippedWeapon;
    public int GetPenetrateCount => _equippedWeapon.PenetrateCount + _bonusPenetrateCount;
    public float GetKillChance => _equippedWeapon.KillChance + _bonusKillChance;

    private void Awake()
    {
        _gameUI = GameUI.instance;
        _playerInput = GetComponent<PlayerInput>();
        _playerController = GetComponent<PlayerController>();
        _animator = GetComponent<Animator>();
        _audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        EquipWeapon(startingWeapon);
    }
    
    private void OnEnable()
    {
        ControllerManager.weaponController = this;
        
        _playerInput.actions["Reload"].performed += HandleReload;
        _playerInput.actions["Slot1"].performed += HandleSlot1;
        _playerInput.actions["Slot2"].performed += HandleSlot2;
        _playerInput.actions["Slot3"].performed += HandleSlot3;
        _playerInput.actions["Grenade"].performed += OnThrowGrenade;
        _playerController.OnCrosshairMove += HandleCrossHairMove;
    }

    private void OnDisable()
    {
        ControllerManager.weaponController = null;
        
        _playerInput.actions["Reload"].performed -= HandleReload;
        _playerInput.actions["Slot1"].performed -= HandleSlot1;
        _playerInput.actions["Slot2"].performed -= HandleSlot2;
        _playerInput.actions["Slot3"].performed -= HandleSlot3;
        _playerInput.actions["Grenade"].performed -= OnThrowGrenade;
        _playerController.OnCrosshairMove -= HandleCrossHairMove;
    }

    private void Update()
    {
        if(_playerInput.actions["Fire"].IsPressed())
            OnFire(new InputAction.CallbackContext());
        
        HandleMouseScroll(Mouse.current.scroll.ReadValue().y);
    }

    private void LateUpdate()
    {
        _equippedWeapon.transform.localPosition =
            Vector3.SmoothDamp(_equippedWeapon.transform.localPosition, Vector3.zero, ref _recoilSmoothDampVelocity, .1f);
        //_recoilAngle = Mathf.SmoothDamp(_recoilAngle, 0, ref _recoilRotationSmoothDampVelocity, .1f);
        //_equippedWeapon.transform.localEulerAngles += Vector3.left * _recoilAngle;

        if (!_isReloading && _equippedWeapon.ProjectileInClip <= 0 && (_equippedWeapon.CurrentProjectileAmount > 0 || _equippedWeapon.InfiniteProjectile))
        {
            Reload();
        }
    }
    
    private void HandleSlot1(InputAction.CallbackContext context)
    {
        if(_isPistolActive)
            EquipWeapon(WeaponType.PISTOL);
    }
    
    private void HandleSlot2(InputAction.CallbackContext context)
    {
        if(_isShogunActive)
            EquipWeapon(WeaponType.SHOTGUN);
    }
    
    private void HandleSlot3(InputAction.CallbackContext context)
    {
        if(_isRifleActive)
            EquipWeapon(WeaponType.RIFLE);
    }
    
    private void HandleReload(InputAction.CallbackContext context)
    {
        if (!_isReloading)
        {
            Reload();
        }
    }
    
    private void HandleCrossHairMove(Vector3 position)
    {
        _crosshairPosition = position; 
    }

    private void HandleMouseScroll(float yValue)
    {
        void Next()
        {
            switch (_equippedWeapon.CurrentWeaponType)
            {
                case WeaponType.PISTOL:
                    if(IsWeaponActive(WeaponType.SHOTGUN))
                        EquipWeapon(WeaponType.SHOTGUN);
                    else if(IsWeaponActive(WeaponType.RIFLE))
                        EquipWeapon(WeaponType.RIFLE);
                    break;
                case WeaponType.SHOTGUN:
                    if(IsWeaponActive(WeaponType.RIFLE))
                        EquipWeapon(WeaponType.RIFLE);
                    else if(IsWeaponActive(WeaponType.PISTOL))
                        EquipWeapon(WeaponType.PISTOL);
                    break;
                case WeaponType.RIFLE:
                    if(IsWeaponActive(WeaponType.PISTOL))
                        EquipWeapon(WeaponType.PISTOL);
                    else if(IsWeaponActive(WeaponType.SHOTGUN))
                        EquipWeapon(WeaponType.SHOTGUN);
                    break;
            }
        }
        
        void Previous()
        {
            switch (_equippedWeapon.CurrentWeaponType)
            {
                case WeaponType.PISTOL:
                    if(IsWeaponActive(WeaponType.RIFLE))
                        EquipWeapon(WeaponType.RIFLE);
                    else if(IsWeaponActive(WeaponType.SHOTGUN))
                        EquipWeapon(WeaponType.SHOTGUN);
                    break;
                case WeaponType.SHOTGUN:
                    if(IsWeaponActive(WeaponType.PISTOL))
                        EquipWeapon(WeaponType.PISTOL);
                    else if(IsWeaponActive(WeaponType.RIFLE))
                        EquipWeapon(WeaponType.RIFLE);
                    break;
                case WeaponType.RIFLE:
                    if(IsWeaponActive(WeaponType.SHOTGUN))
                        EquipWeapon(WeaponType.SHOTGUN);
                    else if(IsWeaponActive(WeaponType.PISTOL))
                        EquipWeapon(WeaponType.PISTOL);
                    break;
            }
        }
        
        switch (yValue)
        {
            case > 0:
                Next();
                break;
            case < 0:
                Previous();
                break;
        }
        
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        bool IsPointerOverUI()
        {
            bool found = false;
            
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Mouse.current.position.ReadValue()
            };
            
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            
            foreach (var result in results)
            {
                if (result.gameObject.GetComponent<Button>() != null)
                    found = true;
            }
            
            return found;
        }
        
        if(!_isReloading && Time.time > _nextShootTime && _equippedWeapon.ProjectileInClip > 0 && !IsPointerOverUI())
            Shoot();
        else if (!_isReloading && _equippedWeapon.ProjectileInClip == 0 && Time.time > _nextEmptyShootTime)
        {
            _nextEmptyShootTime = Time.time + (_equippedWeapon.MsBetweenShots - (_equippedWeapon.MsBetweenShots / 100 * _bonusAttackSpeedPercent)) / 1000;
            _audioManager.PlaySound(_equippedWeapon.EmptyAmmoClip, _equippedWeapon.MuzzlePoint.position);
        }
    }
    
    private void OnThrowGrenade(InputAction.CallbackContext context)
    {
        if( Time.time > _nextThrowingTime && _isGrenadeActive && (grenadeCount > 0 || infiniteGrenade))
            Throw();
    }

    private void Shoot()
    {
        void EjectProjectile()
        {
            foreach (var point in _equippedWeapon.ProjectileSpawnPoint)
            {
                if (point.gameObject.activeSelf)
                {
                    var projectile = BulletPool.Instance.Get(_equippedWeapon.ProjectileType)
                        .SetPosition(point.position)
                        .SetRotation(point.rotation)
                        .SetSpeed(_equippedWeapon.MuzzleVelocity)
                        .SetDamage(_equippedWeapon.Damage + (_equippedWeapon.Damage / 100 * _bonusDamagePercent))
                        .SetCriticalChance(_equippedWeapon.CriticalChance + _bonusCriticalChance)
                        .SetCriticalBonus(_equippedWeapon.CriticalBonus + _bonusCriticalBonus)
                        .SetPeriodDamage(_equippedWeapon.PeriodDamage)
                        .SetPeriodDuration(_equippedWeapon.PeriodDamageDuration)
                        .SetPeriodTick(_equippedWeapon.PeriodDamageTick)
                        .SetKnockBack(_equippedWeapon.KnockBack);
                
                    projectile.gameObject.SetActive(true);
                }
            }

            _equippedWeapon.ProjectileInClip--;
            OnAmmoChange?.Invoke(_equippedWeapon.ProjectileInClip, _equippedWeapon.CurrentProjectileAmount, _equippedWeapon.InfiniteProjectile, _equippedWeapon.CurrentWeaponType);
            _equippedWeapon.MuzzleParticleSystem.Play();
        }
        
        void EjectShellCase()
        {
            var shell = ShellPool.Instance.Get(_equippedWeapon.ShellType)
                .SetPosition(_equippedWeapon.ShellSpawnPoint.position)
                .SetRotation(_equippedWeapon.ShellSpawnPoint.rotation);
                
            shell.gameObject.SetActive(true);
        }

        void Recoil()
        {
            _equippedWeapon.transform.localPosition -= Vector3.forward * _equippedWeapon.RecoilPower;
            //_recoilAngle += 5;
            //_recoilAngle = Mathf.Clamp(_recoilAngle, 0, 15);
        }

        void ShootSound()
        {
            _audioManager.PlaySound(_equippedWeapon.ShootClip, _equippedWeapon.MuzzlePoint.position);
        }
        
        _nextShootTime = Time.time + (_equippedWeapon.MsBetweenShots - (_equippedWeapon.MsBetweenShots / 100 * _bonusAttackSpeedPercent)) / 1000;
        
        _animator?.SetTrigger("Attack");
        
        EjectProjectile();
        EjectShellCase();
        Recoil();
        ShootSound();
    }

    private void Throw()
    {
        _nextThrowingTime = Time.time + grenadeCooldownMs / 1000;

        var distanceToCrosshair = Mathf.Clamp(Vector3.Distance(_crosshairPosition, leftHandHold.position), 0, grenadeMaxThrowingForce);

        if (!doubleGrenade || grenadeCount < 2)
        {
            if(!infiniteGrenade)
                grenadeCount--;
            
            var grenade = GrenadetPool.Instance.Get();
            grenade.gameObject.SetActive(true);
            grenade.Setup(leftHandHold.position, leftHandHold.rotation, transform.forward * distanceToCrosshair + transform.up * grenadeThrowingUpwardForce);
        }
        else if(doubleGrenade && grenadeCount >= 2)
        {
            if(!infiniteGrenade)
                grenadeCount -= 2;

            leftHandHold.transform.Rotate(new Vector3(0, 10, 0));
            
            var grenade = GrenadetPool.Instance.Get();
            grenade.gameObject.SetActive(true);
            grenade.Setup(leftHandHold.position, leftHandHold.rotation, leftHandHold.transform.forward * distanceToCrosshair + leftHandHold.transform.up * grenadeThrowingUpwardForce);
            
            leftHandHold.transform.Rotate(new Vector3(0, -20, 0));
            
            var grenade2 = GrenadetPool.Instance.Get();
            grenade2.gameObject.SetActive(true);
            grenade2.Setup(leftHandHold.position, leftHandHold.rotation, leftHandHold.transform.forward * distanceToCrosshair + leftHandHold.transform.up * grenadeThrowingUpwardForce);
            
            leftHandHold.transform.Rotate(new Vector3(0, 10, 0));
        }
        
        OnAmmoChange?.Invoke(grenadeCount, grenadeMaxCount, infiniteGrenade, WeaponType.GRENADE);
    }
    
    private void Reload()
    {
        IEnumerator AnimateReload()
        {
            OnReloadStart?.Invoke();
            yield return new WaitForSeconds(.2f);

            float reloadSpeed = 1 / (_equippedWeapon.ReloadSpeed - (_equippedWeapon.ReloadSpeed / 100 * _bonusReloadSpeedPercent));
            float percent = 0;
            float maxReloadAngle = 40;

            while (percent < 1)
            {
                if (!_isReloading)
                {
                    _equippedWeapon.transform.localEulerAngles = _initialRotation;
                    OnReloadEnd?.Invoke();
                    OnReloadPercent?.Invoke(0);
                    yield break;
                }
            
                percent += Time.deltaTime * reloadSpeed;
                
                OnReloadPercent?.Invoke(percent);
                
                float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
                float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
                _equippedWeapon.transform.localEulerAngles = _initialRotation + Vector3.left * reloadAngle;
                
                yield return null;
            }
            
            _isReloading = false;
            OnReloadEnd?.Invoke();
            OnReloadPercent?.Invoke(0);
            
            if(_equippedWeapon.CurrentProjectileAmount >= _equippedWeapon.ProjectilePerClip)
            {
                if(!_equippedWeapon.InfiniteProjectile)
                    _equippedWeapon.CurrentProjectileAmount -= _equippedWeapon.ProjectilePerClip - _equippedWeapon.ProjectileInClip;
                
                _equippedWeapon.ProjectileInClip = _equippedWeapon.ProjectilePerClip;
            }
            else
            {
                _equippedWeapon.ProjectileInClip = _equippedWeapon.CurrentProjectileAmount;
                
                if(!_equippedWeapon.InfiniteProjectile)
                    _equippedWeapon.CurrentProjectileAmount -= _equippedWeapon.CurrentProjectileAmount;
            }
            
            OnAmmoChange?.Invoke(_equippedWeapon.ProjectileInClip, _equippedWeapon.CurrentProjectileAmount, _equippedWeapon.InfiniteProjectile, _equippedWeapon.CurrentWeaponType);
        }
        
        if(_equippedWeapon.ProjectileInClip == _equippedWeapon.ProjectilePerClip || (_equippedWeapon.CurrentProjectileAmount <= 0 && !_equippedWeapon.InfiniteProjectile))
            return;

        _isReloading = true;

        StartCoroutine(AnimateReload());
    }
    
    private void EquipWeapon(WeaponType weaponType)
    {
        _isReloading = false;

        if (_equippedWeapon != null)
        {
            _equippedWeapon.transform.localEulerAngles = _initialRotation;
            _equippedWeapon.gameObject.SetActive(false);
        }
        
        switch (weaponType)
        {
            case WeaponType.PISTOL:
                _equippedWeapon = pistol;
                break;
            case WeaponType.SHOTGUN:
                _equippedWeapon = shotgun;
                break;
            case WeaponType.RIFLE:
                _equippedWeapon = rifle;
                break;
        }
        
        _equippedWeapon.gameObject.SetActive(true);
        _initialRotation = _equippedWeapon.transform.localEulerAngles;
        
        OnEquipWeapon?.Invoke(weaponType);
        OnAmmoChange?.Invoke(_equippedWeapon.ProjectileInClip, _equippedWeapon.CurrentProjectileAmount, _equippedWeapon.InfiniteProjectile, _equippedWeapon.CurrentWeaponType);
    }

    public Weapon GetWeapon(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.PISTOL:
                return pistol;
            case WeaponType.SHOTGUN:
                return shotgun;
            case WeaponType.RIFLE:
                return rifle;
            default:
                return pistol;
        }
    }

    public void Aim(Vector3 point)
    {
        _equippedWeapon.transform.LookAt(point);
    }

    public bool IsWeaponActive(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.PISTOL:
                return _isPistolActive;
            case WeaponType.SHOTGUN:
                return _isShogunActive;
            case WeaponType.RIFLE:
                return _isRifleActive;
            case WeaponType.GRENADE:
                return _isGrenadeActive;
        }

        return false;
    }

    public WeaponInfo GetWeaponInfo(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.PISTOL:
                return new WeaponInfo
                {
                    isActive = _isPistolActive,
                    ammoInClip = pistol.ProjectileInClip,
                    ammoPerClip = pistol.ProjectilePerClip,
                    ammoCurrent = pistol.CurrentProjectileAmount,
                    ammoMax = pistol.MaxProjectileAmount,
                    bonusMaxClip = _bonusMaxClip,
                    bonusTakeClip = _bonusTakeClip
                };
            case WeaponType.SHOTGUN:
                return new WeaponInfo
                {
                    isActive = _isShogunActive,
                    ammoInClip = shotgun.ProjectileInClip,
                    ammoPerClip = shotgun.ProjectilePerClip,
                    ammoCurrent = shotgun.CurrentProjectileAmount,
                    ammoMax = shotgun.MaxProjectileAmount,
                    bonusMaxClip = _bonusMaxClip,
                    bonusTakeClip = _bonusTakeClip
                };
            case WeaponType.RIFLE:
                return new WeaponInfo
                {
                    isActive = _isRifleActive,
                    ammoInClip = rifle.ProjectileInClip,
                    ammoPerClip = rifle.ProjectilePerClip,
                    ammoCurrent = rifle.CurrentProjectileAmount,
                    ammoMax = rifle.MaxProjectileAmount,
                    bonusMaxClip = _bonusMaxClip,
                    bonusTakeClip = _bonusTakeClip
                };
            case WeaponType.GRENADE:
                return new WeaponInfo
                {
                    ammoInClip = 1,
                    ammoPerClip = 1,
                    ammoCurrent = grenadeCount,
                    ammoMax = grenadeMaxCount
                };
            default:
                return new WeaponInfo();
        }
    }

    public WeaponType GetRandomWeaponType()
    {
        Array values = Enum.GetValues(typeof(WeaponType));
        System.Random random = new System.Random();
        return (WeaponType)values.GetValue(random.Next(values.Length));
    }
    
    public void AddAmmo(int count, WeaponType weaponType, bool activateWeapon, bool isCountPercent = false)
    {
        switch (weaponType)
        {
            case WeaponType.SHOTGUN:
                if (activateWeapon && !_isShogunActive)
                {
                    _isShogunActive = true;
                    _gameUI.SetSlot(2, true);
                }
                
                var percentShotgunCount = Convert.ToInt32(Math.Round((double)shotgun.MaxProjectileAmount / 100 * (double)count, 0));
                shotgun.CurrentProjectileAmount += isCountPercent ? percentShotgunCount : count;
                
                if (shotgun.CurrentProjectileAmount > shotgun.MaxProjectileAmount)
                    shotgun.CurrentProjectileAmount = shotgun.MaxProjectileAmount;
                
                OnAmmoChange?.Invoke(shotgun.ProjectileInClip, shotgun.CurrentProjectileAmount, shotgun.InfiniteProjectile, WeaponType.SHOTGUN);
                break;
            case WeaponType.RIFLE:
                if (activateWeapon && !_isRifleActive)
                {
                    _isRifleActive = true;
                    _gameUI.SetSlot(3, true);
                }
                
                var percentRifleCount = Convert.ToInt32(Math.Round((double)rifle.MaxProjectileAmount / 100 * (double)count, 0));
                rifle.CurrentProjectileAmount += isCountPercent ? percentRifleCount : count;
                
                if (rifle.CurrentProjectileAmount > rifle.MaxProjectileAmount)
                    rifle.CurrentProjectileAmount = rifle.MaxProjectileAmount;
                
                OnAmmoChange?.Invoke(rifle.ProjectileInClip, rifle.CurrentProjectileAmount, rifle.InfiniteProjectile, WeaponType.RIFLE);
                break;
            case WeaponType.GRENADE:
                if (activateWeapon && !_isGrenadeActive)
                {
                    _isGrenadeActive = true;
                    _gameUI.SetSlot(4, true);
                }

                var percentCount = Convert.ToInt32(Math.Round((double)grenadeMaxCount / 100 * (double)count, 0));
                grenadeCount += isCountPercent ? percentCount : count;

                if (grenadeCount > grenadeMaxCount)
                    grenadeCount = grenadeMaxCount;
                
                OnAmmoChange?.Invoke(grenadeCount, grenadeMaxCount, infiniteGrenade, WeaponType.GRENADE);
                break;
        }
    }

    public void AddBonusDamagePercent(float value)
    {
        _bonusDamagePercent += value;
    }
    
    public void AddBonusAttackSpeedPercent(float value)
    {
        _bonusAttackSpeedPercent += value;
    }
    
    public void AddBonusReloadSpeedPercent(float value)
    {
        _bonusReloadSpeedPercent += value;
    }
    
    public void AddBonusCriticalChance(float value)
    {
        _bonusCriticalChance += value;
    }
    
    public void AddBonusCriticalBonus(float value)
    {
        _bonusCriticalBonus += value;
    }
    
    public void AddBonusMaxClip(int value)
    {
        _bonusMaxClip += value;
        
        shotgun.MaxProjectileAmount += shotgun.ProjectilePerClip * _bonusMaxClip;
        rifle.MaxProjectileAmount += rifle.ProjectilePerClip * _bonusMaxClip;

        if (_halfMaxClip)
        {
            shotgun.MaxProjectileAmount /= 2;
            rifle.MaxProjectileAmount /= 2;
        }
        
        if (_doubleMaxClip)
        {
            shotgun.MaxProjectileAmount *= 2;
            rifle.MaxProjectileAmount *= 2;
        }
    }
    
    public void AddBonusTakeClip(int value)
    {
        _bonusTakeClip += value;
    }
    
    public void AddPenetrateCount(int value)
    {
        _bonusPenetrateCount += value;
    }
    
    public void AddKillChance(float value)
    {
        _bonusKillChance += value;
    }

    public void SetDoubleGrenade(bool value)
    {
        doubleGrenade = value;
    }
    
    public void SetHalfMaxAmmo(bool value)
    {
        _halfMaxClip = value;
    }
    
    public void SetDoubleMaxAmmo(bool value)
    {
        _doubleMaxClip = value;
    }
}

public struct WeaponInfo
{
    public bool isActive;
    public int ammoInClip;
    public int ammoPerClip;
    public int ammoCurrent;
    public int ammoMax;
    public int bonusMaxClip;
    public int bonusTakeClip;

    public int GetAmmoPerClip()
    {
        return ammoPerClip * (bonusTakeClip > 0 ? bonusTakeClip + 1 : 1);
    }
}