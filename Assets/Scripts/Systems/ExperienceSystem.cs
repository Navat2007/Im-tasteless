using System;
using UnityEngine;

public class ExperienceSystem : MonoBehaviour
{
    public event Action<float> OnXpChange;
    public event Action<float> OnNextLevelXpChange;
    public event Action<int> OnLevelChange;
    
    [field:Header("XP")]
    [field: SerializeField] public int Level { get; private set; }
    [field: SerializeField] public int MaxLevel { get; private set; }
    [field: SerializeField] public float Xp { get; private set; }
    [field: SerializeField] public float NextLevelXp { get; private set; }

    [Header("На сколько увеличивается опыт для след. уровня")] 
    [SerializeField] private float nextLevelAdditive = 1000;

    private float _bonusExperiencePercent;
    
    private void OnEnable()
    {
        ControllerManager.experienceSystem = this;
    }

    private void OnDisable()
    {
        ControllerManager.experienceSystem = null;
    }

    private void Start()
    {
        OnNextLevelXpChange?.Invoke(NextLevelXp);
        OnXpChange?.Invoke(Xp);
    }

    public void AddXp(float amount)
    {
        Xp += amount + (amount / 100 * _bonusExperiencePercent);

        if (Xp >= NextLevelXp)
        {
            LevelUp();
        }
        
        OnXpChange?.Invoke(Xp);
    }

    public void AddBonusXpPercent(float value)
    {
        _bonusExperiencePercent += value;
    }
    
    public void AddMaxLevel(int value)
    {
        MaxLevel += value;
    }

    [ContextMenu("Level Up")]
    private void LevelUp()
    {
        Level++;

        Xp = 0;
        NextLevelXp += nextLevelAdditive;
        
        OnLevelChange?.Invoke(Level);
        OnNextLevelXpChange?.Invoke(NextLevelXp);
        OnXpChange?.Invoke(Xp);
    }
}
