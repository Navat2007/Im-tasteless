using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceSystem : MonoBehaviour
{
    public event Action<int, int> OnXpChange;
    public event Action<int> OnLevelChange;
    
    [field:Header("XP")]
    [field: SerializeField] public int Level { get; private set; }
    [field: SerializeField] public int Xp { get; private set; }
    [field: SerializeField] public int NextLevelXp { get; private set; }

    private void Start()
    {
        OnXpChange?.Invoke(Xp, NextLevelXp);
    }

    public void AddXp(int amount)
    {
        //print($"add xp {amount}");
        
        Xp += amount;

        if (Xp >= NextLevelXp)
        {
            LevelUp();
        }
        
        OnXpChange?.Invoke(Xp, NextLevelXp);
    }

    [ContextMenu("Level Up")]
    private void LevelUp()
    {
        Level++;

        Xp = 0;
        NextLevelXp += 10;
        
        OnLevelChange?.Invoke(Level);
    }
}
