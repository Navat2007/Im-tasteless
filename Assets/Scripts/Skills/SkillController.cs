using System;
using System.Collections.Generic;
using Skills;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkillController : MonoBehaviour
{
    [Header("Обычные")]
    [SerializeField] private float commonSkillChance = 62;
    [SerializeField] private int commonSkillMaxCount = 100;
    [SerializeField] private List<Skill> commonSkills = new();
    
    [Header("Необычные")]
    [SerializeField] private float uncommonSkillChance = 25;
    [SerializeField] private int uncommonSkillMaxCount = 50;
    [SerializeField] private List<Skill> uncommonSkills = new();
    
    [Header("Редкие")]
    [SerializeField] private float rareSkillChance = 10;
    [SerializeField] private int rareSkillMaxCount = 10;
    [SerializeField] private List<Skill> rareSkills = new();
    
    [Header("Уникальные")]
    [SerializeField] private float uniqueSkillChance = 3;
    [SerializeField] private int uniqueSkillMaxCount = 1;
    [SerializeField] private List<Skill> uniqueSkills = new();

    private int _commonSkillCount;
    private int _uncommonSkillCount;
    private int _rareSkillCount;
    private int _uniqueSkillCount;

    private List<Skill> _currentChoiceList = new ();
    private List<Skill> _currentSkillsList = new ();

    private void OnEnable()
    {
        ControllerManager.skillController = this;
    }

    private void OnDisable()
    {
        ControllerManager.skillController = null;
    }

    public List<Skill> GetCommonSkills => commonSkills;
    public List<Skill> GetUncommonSkills => uncommonSkills;
    public List<Skill> GetRareSkills => rareSkills;
    public List<Skill> GetUniqueSkills => uniqueSkills;

    public void ResetChoiceList() => _currentChoiceList.Clear();
    
    public Skill GetRandomSkill()
    {
        bool IsAvailableToAdd(Skill skill)
        {
            if (_currentChoiceList.Contains(skill)) return false;
            if (_currentSkillsList.Contains(skill) && skill.GetCurrentLevel >= skill.GetMaxLevel) return false;
            
            switch (skill.GetSkillRarity)
            {
                case SkillRarity.COMMON:
                    if (_commonSkillCount >= commonSkillMaxCount) return false;
                    if (!Helper.IsCritical(commonSkillChance)) return false;
                    break;
                case SkillRarity.UNCOMMON:
                    if (_uncommonSkillCount >= uncommonSkillMaxCount) return false;
                    if (!Helper.IsCritical(uncommonSkillChance)) return false;
                    break;
                case SkillRarity.RARE:
                    if (_rareSkillCount >= rareSkillMaxCount) return false;
                    if (!Helper.IsCritical(rareSkillChance)) return false;
                    break;
                case SkillRarity.UNIQUE:
                    if (_uniqueSkillCount >= uniqueSkillMaxCount) return false;
                    if (!Helper.IsCritical(uniqueSkillChance)) return false;
                    break;
            }

            return true;
        }

        int count = 0;

        while (count < 2000)
        {
            Array values = Enum.GetValues(typeof(SkillRarity));
            System.Random random = new System.Random();
            SkillRarity randomSkillRarity = (SkillRarity)values.GetValue(random.Next(values.Length));
            
            var skill = GetRandomSkill(randomSkillRarity);
            
            if (skill != null && IsAvailableToAdd(skill))
            {
                _currentChoiceList.Add(skill);
                return skill;
            }

            count++;
        }

        return null;
    }

    private Skill GetRandomSkill(SkillRarity skillRarity)
    {
        switch (skillRarity)
        {
            case SkillRarity.COMMON:
                return commonSkills.Count == 0 ? null : commonSkills[Random.Range(0, commonSkills.Count)];
            case SkillRarity.UNCOMMON:
                return uncommonSkills.Count == 0 ? null :  uncommonSkills[Random.Range(0, uncommonSkills.Count)];
            case SkillRarity.RARE:
                return rareSkills.Count == 0 ? null :  rareSkills[Random.Range(0, rareSkills.Count)];
            case SkillRarity.UNIQUE:
                return uniqueSkills.Count == 0 ? null :  uniqueSkills[Random.Range(0, uniqueSkills.Count)];
            default:
                return commonSkills.Count == 0 ? null :  commonSkills[Random.Range(0, commonSkills.Count)];
        }
    }

    private bool IsInSkillsList(Skill skill) => _currentSkillsList.Contains(skill);

    public void AddToSkillsList(Skill skill)
    {
        switch (skill.GetSkillRarity)
        {
            case SkillRarity.COMMON:
                _commonSkillCount++;
                break;
            case SkillRarity.UNCOMMON:
                _uncommonSkillCount++;
                break;
            case SkillRarity.RARE:
                _rareSkillCount++;
                break;
            case SkillRarity.UNIQUE:
                _uniqueSkillCount++;
                break;
        }

        if (!IsInSkillsList(skill))
        {
            _currentSkillsList.Add(skill);
            skill.Activate();
        }
        else
        {
            _currentSkillsList.Find(item => item.GetID == skill.GetID).Activate();
        }
    }
}
