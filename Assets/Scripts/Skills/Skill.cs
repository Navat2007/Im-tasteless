using System;
using System.Collections.Generic;
using UnityEngine;

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
        [SerializeField] protected List<Skill> exceptedSkills;

        private void Awake()
        {
            if (maxLevel == 0) maxLevel++;
        }

        public virtual void Activate()
        {
            foreach (var skill in exceptedSkills)
            {
                ControllerManager.skillController.AddToExceptedList(skill.GetID);
            }
        }

        public int GetID => id;
        public Sprite GetImage => skillImage;
        public string GetName => skillName;
        public string GetDescription => skillDescription;
        public SkillRarity GetSkillRarity => skillRarity;
        public int GetMaxLevel => maxLevel;
        public List<Skill> GetExceptedSkills => exceptedSkills;
    }

    public enum SkillRarity
    {
        COMMON,
        UNCOMMON,
        RARE,
        UNIQUE
    }
}