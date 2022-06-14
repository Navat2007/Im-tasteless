using System;
using System.Collections;
using System.Collections.Generic;
using Pools;
using UnityEngine;

[RequireComponent(typeof(AnimationController))]
public class HealthSystem : MonoBehaviour
{
    public event Action<GameObject, ProjectileHitInfo> OnDeath;
    public event Action<float> OnHealthChange;
    public event Action<float> OnMaxHealthChange;
    public event Action<float> OnHealed;
    public event Action<float, float, float> OnTakeDamage;
    public event Action<int> OnArmorChange;
    
    [field: Header("Настройки здоровья")] 
    [field: SerializeField] public float CurrentHealth { get; private set; }
    [field: SerializeField] public float MaxHealth { get; private set; }
    [field: SerializeField] public float HealthInSecond { get; private set; }
    [field: SerializeField] public int Armor { get; private set; }
    [field: SerializeField] public float InvulnerabilityTime { get; private set; }

    [Header("Настройки эффекта вспышки при получении урона")]
    [SerializeField] private float blinkDuration = 0.1f;

    [Header("Настройки всплывающий текст")] 
    [SerializeField] private GameObject floatingTextPrefab;

    private AnimationController _animationController;
    private Renderer _renderer;
    private Color _startColor;
    private bool _isOverTimeHealActive;
    private bool _isDeath;
    private float _nextInvulnerabilityTime;
    private float _nextHealthInSecondTickTime;
    private Dictionary<PeriodDamageSource, ProjectileHitInfo> _periodDamages = new ();
    private Dictionary<PeriodDamageSource, float> _periodDamageTimers = new ();
    private Dictionary<PeriodDamageSource, float> _periodDamageDurations = new ();

    private float _bonusHealPercent;

    private void Awake()
    {
        _animationController = GetComponent<AnimationController>();
    }
    
    private void OnEnable()
    {
        _isDeath = false;
        _isOverTimeHealActive = false;
        
        AddHealth(MaxHealth);
    }

    private void Update()
    {
        if (Time.time > _nextHealthInSecondTickTime)
        {
            _nextHealthInSecondTickTime = Time.time + 1;
            
            if(ControllerManager.playerController != null && ControllerManager.playerController.GetMoveVelocity == Vector3.zero)
                AddHealth(HealthInSecond);
        }
        
        foreach (var item in _periodDamages)
        {
            if (_periodDamageDurations[item.Key] > 0)
            {
                var timer = _periodDamageTimers[item.Key];
                if (Time.time > timer)
                {
                    _periodDamageTimers[item.Key] = Time.time + item.Value.periodDamageStruct.tick;
                    _periodDamageDurations[item.Key] -= item.Value.periodDamageStruct.tick;
                    
                    TakeDamage(new ProjectileHitInfo
                    {
                        damage = item.Value.periodDamageStruct.damage,
                        isCritical = item.Value.isCritical,
                        criticalBonus = item.Value.criticalBonus,
                        hitDirection = item.Value.hitDirection,
                        hitPoint = transform.position,
                        periodDamageStruct = new PeriodDamageStruct()
                    }, false);
                }
            }
            else
            {
                _periodDamages.Remove(item.Key);
                _periodDamageTimers.Remove(item.Key);
                _periodDamageDurations.Remove(item.Key);
                break;
            }
        }
    }

    public void Init(float value)
    {
        MaxHealth = value;
        AddHealth(MaxHealth);
        OnMaxHealthChange?.Invoke(MaxHealth);
        OnHealthChange?.Invoke(CurrentHealth);
    }

    public void SetRender(Renderer newRenderer)
    {
        _renderer = newRenderer;
        _startColor = _renderer.material.color;
    }

    public float GetPercentCurrentMax()
    {
        var percent = CurrentHealth * 100 / MaxHealth;
        return percent;
    }

    public void AddHealth(float amount)
    {
        var prevHealth = CurrentHealth;
        
        CurrentHealth += amount + (amount / 100 * _bonusHealPercent);

        if (CurrentHealth > MaxHealth)
            CurrentHealth = MaxHealth;
        
        OnHealed?.Invoke(CurrentHealth);
        
        if(prevHealth < MaxHealth)
            OnHealthChange?.Invoke(CurrentHealth);
    }

    public void AddHealth(float amount, float tickTimePeriod, int tickAmount)
    {
        if(_isOverTimeHealActive)
            StopCoroutine(HealOverTime(amount, tickTimePeriod, tickAmount));

        StartCoroutine(HealOverTime(amount, tickTimePeriod, tickAmount));
    }
    
    public void AddHealthPercent(float percent)
    {
        var prevHealth = CurrentHealth;
        
        CurrentHealth += MaxHealth / 100 * percent;

        if (CurrentHealth > MaxHealth)
            CurrentHealth = MaxHealth;
        
        OnHealed?.Invoke(CurrentHealth);
        
        if(prevHealth < MaxHealth)
            OnHealthChange?.Invoke(CurrentHealth);
    }

    public void AddMaxHealth(float value)
    {
        MaxHealth += value;

        OnMaxHealthChange?.Invoke(MaxHealth);
        
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
            OnHealthChange?.Invoke(CurrentHealth);
        }
    }
    
    public void AddMaxHealthPercent(float percent)
    {
        MaxHealth += MaxHealth / 100 * percent;

        OnMaxHealthChange?.Invoke(MaxHealth);
        
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
            OnHealthChange?.Invoke(CurrentHealth);
        }
    }

    private IEnumerator HealOverTime(float amount, float tickTimePeriod, int tickAmount)
    {
        _isOverTimeHealActive = true;
        
        int count = 0;

        while (count < tickAmount)
        {
            AddHealth(amount);
            count++;

            yield return new WaitForSeconds(tickTimePeriod);
        }
        
        _isOverTimeHealActive = false;
    }
    
    public void AddArmor(int amount)
    {
        Armor += amount;

        if (Armor > 1)
            Armor = 1;
        
        OnArmorChange?.Invoke(Armor);
    }
    
    public void AddHealthInSecond(float amount)
    {
        HealthInSecond += amount;
    }

    public void AddNextInvulnerabilityTime(float time)
    {
        _nextInvulnerabilityTime = Time.time + time;
    }

    public void AddBonusHealPercent(float value)
    {
        _bonusHealPercent += value;
    }
    
    public void TakeDamage(ProjectileHitInfo projectileHitInfo, bool playHitAnimation = true)
    {
        //TODO make refactor
        IEnumerator Blink(float time)
        {
            if (_renderer == null)
            {
                var renders = gameObject.GetComponentsInChildren<Renderer>();

                foreach (var item in renders)
                {
                    if (!item.gameObject.activeSelf) continue;
                    
                    _renderer = item;
                    _startColor = _renderer.material.color;
                    break;
                }
            }
            
            _renderer.material.color = Color.white * 10;
            yield return new WaitForSeconds(time);
            _renderer.material.color = _startColor * 1;
        }
        
        if(_isDeath) return;
        if(Time.time < _nextInvulnerabilityTime) return;
        
        StopCoroutine(Blink(blinkDuration));
        StartCoroutine(Blink(blinkDuration));

        if(playHitAnimation)
            _animationController.SetState(AnimationState.HIT);

        if (projectileHitInfo.isCritical)
        {
            projectileHitInfo.MakeDamageCritical();
            projectileHitInfo.MakePeriodDamageCritical();
        }

        if (projectileHitInfo.periodDamageStruct.damage > 0)
        {
            if (!_periodDamages.ContainsKey(projectileHitInfo.periodDamageStruct.source))
            {
                _periodDamages.Add(projectileHitInfo.periodDamageStruct.source, projectileHitInfo);
                _periodDamageTimers.Add(projectileHitInfo.periodDamageStruct.source, Time.time + projectileHitInfo.periodDamageStruct.tick);
                _periodDamageDurations.Add(projectileHitInfo.periodDamageStruct.source, projectileHitInfo.periodDamageStruct.duration);
            }
        }

        if (floatingTextPrefab != null)
        {
            var floatingText = FloatingTextPool.Instance.Get();
           floatingText.Setup(Math.Round(projectileHitInfo.damage, 1).ToString(), projectileHitInfo.isCritical, Armor > 0, transform.position);
           floatingText.gameObject.SetActive(true);
        }

        if (Armor > 0)
        {
            Armor = 0;
            OnArmorChange?.Invoke(Armor);
            OnTakeDamage?.Invoke(projectileHitInfo.damage, CurrentHealth, MaxHealth);
            return;
        }

        CurrentHealth -= projectileHitInfo.damage;
        
        if (CurrentHealth < 0)
            CurrentHealth = 0;
        
        OnHealthChange?.Invoke(CurrentHealth);
        OnTakeDamage?.Invoke(projectileHitInfo.damage, CurrentHealth, MaxHealth);

        AddNextInvulnerabilityTime(InvulnerabilityTime);

        if (CurrentHealth <= 0) Die(projectileHitInfo);
    }

    [ContextMenu("Убить себя")]
    private void Die(ProjectileHitInfo projectileHitInfo)
    {
        _isDeath = true;
        
        _renderer.material.color = _startColor;

        OnDeath?.Invoke(gameObject, projectileHitInfo);
    }

    #region Test

    [ContextMenu("Получить 10 урона")]
    private void Take10Damage()
    {
        TakeDamage(new ProjectileHitInfo
        {
            damage = 10
        });
    }
    
    [ContextMenu("Получить 50 урона")]
    private void Take50Damage()
    {
        TakeDamage(new ProjectileHitInfo
        {
            damage = 50
        });
    }

    #endregion
}

public struct PeriodDamageStruct
{
    public PeriodDamageSource source;
    public float damage;
    public float duration;
    public float tick;
}

public enum PeriodDamageSource
{
    PROJECTILE,
    GRENADE,
    ABILITY
}
