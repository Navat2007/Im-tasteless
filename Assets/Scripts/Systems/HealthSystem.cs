using System;
using System.Collections;
using Interface;
using TMPro;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event Action OnDeath;
    public event Action<float, float> OnHealthChange;
    public event Action<int> OnArmorChange;
    
    [field: Header("Настройки здоровья")] 
    [field: SerializeField] public float CurrentHealth { get; private set; }
    [field: SerializeField] public float MaxHealth { get; private set; }
    [field: SerializeField] public int Armor { get; private set; }

    [Header("Настройки эффекта вспышки при получении урона")]
    [SerializeField] private float blinkIntensity = 10;
    [SerializeField] private float blinkDuration = 0.1f;
    [SerializeField] private float blinkTimer;

    [Header("Настройки при смерти")] 
    [SerializeField] private bool isDestroyOnDeath;
    [SerializeField] private ParticleSystem deathEffect;

    [Header("Настройки всплывающий текст")] 
    [SerializeField] private GameObject floatingTextPrefab;

    private MeshRenderer _meshRenderer;
    private Color _startColor;
    private const float DefaultHealth = 1;
    private bool _isOverTimeHealActive;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
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
        {
            _meshRenderer.material.color = _startColor;
        }
    }

    private void Init(float value)
    {
        MaxHealth = value;
        AddHealth(MaxHealth);
    }

    public void AddHealth(float amount)
    {
        CurrentHealth += amount;

        if (CurrentHealth > MaxHealth)
            CurrentHealth = MaxHealth;
        
        OnHealthChange?.Invoke(CurrentHealth, MaxHealth);
    }

    public void AddHealth(float amount, float tickTimePeriod, int tickAmount)
    {
        if(_isOverTimeHealActive)
            StopCoroutine(HealOverTime(amount, tickTimePeriod, tickAmount));

        StartCoroutine(HealOverTime(amount, tickTimePeriod, tickAmount));
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

    public void TakeHit(float amount, bool isCritical, Vector3 hitPoint, Vector3 hitDirection)
    {
        //print($"take hit {amount}");
        if (amount >= CurrentHealth && deathEffect != null)
        {
            Destroy(
                Instantiate(
                    deathEffect.gameObject, 
                    hitPoint, 
                    Quaternion.FromToRotation(Vector3.forward, hitDirection)),
                    deathEffect.main.startLifetime.constantMax 
                );
        }
        
        TakeDamage(amount, isCritical);
    }
    
    public void TakeDamage(float amount, bool isCritical)
    {
        blinkTimer = blinkDuration;
        
        amount = isCritical ? amount * 2 : amount;

        if (floatingTextPrefab != null)
        {
            GameObject floatingTextGameObject = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            if (floatingTextGameObject.gameObject.TryGetComponent(out FloatingText floatingText))
            {
                floatingText.Setup(amount.ToString(), isCritical, Armor > 0);
            }
        }

        if (Armor > 0)
        {
            Armor = 0;
            OnArmorChange?.Invoke(Armor);
            return;
        }

        CurrentHealth -= amount;
        
        if (CurrentHealth < 0)
            CurrentHealth = 0;
        
        OnHealthChange?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0) Die();
    }

    [ContextMenu("Убить себя")]
    private void Die()
    {
        OnDeath?.Invoke();
        
        if(isDestroyOnDeath)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }

    [ContextMenu("Получить 10 урона")]
    private void Take10Damage()
    {
        TakeDamage(10, false);
    }
    
    [ContextMenu("Получить 50 урона")]
    private void Take50Damage()
    {
        TakeDamage(50, false);
    }
}
