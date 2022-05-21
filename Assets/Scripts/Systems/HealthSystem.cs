using System;
using System.Collections;
using Interface;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event Action<ProjectileHitInfo> OnDeath;
    public event Action<float> OnHealthChange;
    public event Action<float> OnMaxHealthChange;
    public event Action<float, float, float> OnTakeDamage;
    public event Action<int> OnArmorChange;
    
    [field: Header("Настройки здоровья")] 
    [field: SerializeField] public float CurrentHealth { get; private set; }
    [field: SerializeField] public float MaxHealth { get; private set; }
    [field: SerializeField] public float HealthInSecond { get; private set; }
    [field: SerializeField] public int Armor { get; private set; }
    [field: SerializeField] public float InvulnerabilityTime { get; private set; }

    [Header("Настройки эффекта вспышки при получении урона")]
    [SerializeField] private float blinkIntensity = 10;
    [SerializeField] private float blinkDuration = 0.1f;
    [SerializeField] private float blinkTimer;

    [Header("Настройки всплывающий текст")] 
    [SerializeField] private GameObject floatingTextPrefab;

    private MeshRenderer _meshRenderer;
    private Color _startColor;
    private const float DefaultHealth = 1;
    private bool _isOverTimeHealActive;
    private bool _isDeath;
    private bool _isPlayer;
    private float _nextInvulnerabilityTime;
    private float _nextHealthInSecondTickTime;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _isPlayer = GetComponent<Player>() != null;
    }
    
    private void OnEnable()
    {
        AddHealth(MaxHealth);
    }

    private void Start()
    {
        _startColor = _meshRenderer.material.color;
        
        IHealth health = GetComponent<IHealth>();
        if (health != null)
        {
            Init(health.Health);
        }
        else
        {
            Init(DefaultHealth);
        }
    }

    private void Update()
    {
        if (blinkTimer > 0)
        {
            blinkTimer -= Time.deltaTime;
            float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
            float intensity = (lerp * blinkIntensity) + 1;
            _meshRenderer.material.color = Color.white * intensity;
        }
        else
            _meshRenderer.material.color = _startColor;

        if (Time.time > _nextHealthInSecondTickTime)
        {
            _nextHealthInSecondTickTime = Time.time + 1;
            
            if(ControllerManager.playerController != null && ControllerManager.playerController.GetMoveVelocity == Vector3.zero)
                AddHealth(HealthInSecond);
        }
    }

    public void Init(float value)
    {
        MaxHealth = value;
        AddHealth(MaxHealth);
        OnMaxHealthChange?.Invoke(MaxHealth);
        OnHealthChange?.Invoke(CurrentHealth);
    }

    public void AddHealth(float amount)
    {
        var prevHealth = CurrentHealth;
        
        CurrentHealth += amount;

        if (CurrentHealth > MaxHealth)
            CurrentHealth = MaxHealth;
        
        if(prevHealth < MaxHealth)
            OnHealthChange?.Invoke(CurrentHealth);
    }

    public void AddHealth(float amount, float tickTimePeriod, int tickAmount)
    {
        if(_isOverTimeHealActive)
            StopCoroutine(HealOverTime(amount, tickTimePeriod, tickAmount));

        StartCoroutine(HealOverTime(amount, tickTimePeriod, tickAmount));
    }

    public void AddMaxHealth(float value)
    {
        MaxHealth += value;
        OnMaxHealthChange?.Invoke(MaxHealth);
    }
    
    public void AddMaxHealthPercent(float percent)
    {
        MaxHealth += MaxHealth / 100 * percent;
        OnMaxHealthChange?.Invoke(MaxHealth);
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
    
    public void TakeDamage(ProjectileHitInfo projectileHitInfo)
    {
        if(_isDeath) return;
        if(Time.time < _nextInvulnerabilityTime && _isPlayer) return;
        
        blinkTimer = blinkDuration;
        
        if(projectileHitInfo.isCritical)
            projectileHitInfo.MakeDamageCritical();

        if (floatingTextPrefab != null)
        {
            GameObject floatingTextGameObject = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            if (floatingTextGameObject.gameObject.TryGetComponent(out FloatingText floatingText))
            {
                floatingText.Setup(projectileHitInfo.damage.ToString(), projectileHitInfo.isCritical, Armor > 0);
            }
        }

        if (Armor > 0)
        {
            Armor = 0;
            OnArmorChange?.Invoke(Armor);
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
        
        blinkTimer = 0;
        _meshRenderer.material.color = _startColor;
        
        OnDeath?.Invoke(projectileHitInfo);
    }

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
}
