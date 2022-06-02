using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ExperiencePerLevelStruct
{
    public int level;
    public float xp;
}

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

    [Header("Кол-во опыта для уровня")] 
    [SerializeField] private List<float> levelList = new();

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
        if (levelList.Count == 0) throw new Exception("Player => ExperienceSystem => Опыт для уровней не назначен");

        Level = 1;
        Xp = 0;
        NextLevelXp = levelList[0];
        //MaxLevel = levelList.Count + 1;
        OnNextLevelXpChange?.Invoke(NextLevelXp);
        OnXpChange?.Invoke(Xp);
    }

    public void AddXp(float amount)
    {
        if (Level >= MaxLevel) return;
        
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
        if (Level >= MaxLevel) return;
        
        Level++;

        Xp = 0;

        if (Level >= levelList.Count + 1)
            NextLevelXp = 0;
        else
            NextLevelXp = levelList[Level - 1];
        
        OnLevelChange?.Invoke(Level);
        OnNextLevelXpChange?.Invoke(NextLevelXp);
        OnXpChange?.Invoke(Xp);
    }
}
