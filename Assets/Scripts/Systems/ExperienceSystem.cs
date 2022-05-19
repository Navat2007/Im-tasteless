using System;
using UnityEngine;

public class ExperienceSystem : MonoBehaviour
{
    public event Action<float, int> OnXpChange;
    public event Action<int> OnLevelChange;
    
    [field:Header("XP")]
    [field: SerializeField] public int Level { get; private set; }
    [field: SerializeField] public int MaxLevel { get; private set; }
    [field: SerializeField] public float Xp { get; private set; }
    [field: SerializeField] public int NextLevelXp { get; private set; }

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
        OnXpChange?.Invoke(Xp, NextLevelXp);
    }

    public void AddXp(float amount)
    {
        Xp += amount + (amount / 100 * _bonusExperiencePercent);

        if (Xp >= NextLevelXp)
        {
            LevelUp();
        }
        
        OnXpChange?.Invoke(Xp, NextLevelXp);
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
        NextLevelXp += 1000;
        
        OnLevelChange?.Invoke(Level);
    }
}
