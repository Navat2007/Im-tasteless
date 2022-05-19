using System;
using UnityEngine;
using UnityEngine.UI;

namespace Skills
{
    public abstract class Skill : MonoBehaviour
    {
        [SerializeField] protected int id;
        [SerializeField] protected Sprite skillImage;
        [SerializeField] protected string skillName;
        [SerializeField] protected string skillDescription;
        [SerializeField] protected SkillRarity skillRarity;
        [SerializeField] protected int maxLevel;

        protected Action onActivate;
        protected int currentLevel;
    
        public abstract void Activate();

        public int GetID => id;
        public Sprite GetImage => skillImage;
        public string GetName => skillName;
        public string GetDescription => skillDescription;
        public SkillRarity GetSkillRarity => skillRarity;
        public int GetCurrentLevel => currentLevel;
        public int GetMaxLevel => maxLevel;
    }

    public enum SkillRarity
    {
        COMMON,
        UNCOMMON,
        RARE,
        UNIQUE
    }
}