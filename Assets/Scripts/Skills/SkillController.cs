using System;
using System.Collections.Generic;
using Skills;
using Skills.uncommon;
using UnityEngine;

public struct SkillStruct
{
    public int id;
    public string title;
    public int level;
    public Skill skill;
}

public class SkillController : MonoBehaviour
{
    [field: SerializeField] public bool IsNextDouble { get; private set; }
    
    [Header("Скилы когда закончились остальные")] 
    [SerializeField] private Skill lastSkill;
    
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

    private List<Skill> _currentChoiceList = new ();
    private List<SkillStruct> _currentSkillsList = new ();

    private void OnEnable()
    {
        ControllerManager.skillController = this;
    }

    private void OnDisable()
    {
        _currentChoiceList.Clear();
        _currentSkillsList.Clear();
        ControllerManager.skillController = null;
    }

    public List<Skill> GetCommonSkills => commonSkills;
    public List<Skill> GetUncommonSkills => uncommonSkills;
    public List<Skill> GetRareSkills => rareSkills;
    public List<Skill> GetUniqueSkills => uniqueSkills;

    public void ResetChoiceList() => _currentChoiceList.Clear();

    public SkillStruct GetSkillByID(int id)
    {
        return _currentSkillsList.Find(skill => skill.skill.GetID == id);
    }
    
    public Skill GetRandomSkill()
    {
        bool IsAvailableToAdd(Skill skill)
        {
            if (_currentChoiceList.Contains(skill)) return false;
            if (_currentSkillsList.Exists(item => item.skill.GetID == skill.GetID) 
                && _currentSkillsList.Find(item => item.skill.GetID == skill.GetID).level >= skill.GetMaxLevel) return false;
            
            switch (skill.GetSkillRarity)
            {
                case SkillRarity.COMMON:
                    //if (_commonSkillCount >= commonSkillMaxCount) return false;
                    if (!Helper.IsCritical(commonSkillChance)) return false;
                    break;
                case SkillRarity.UNCOMMON:
                    //if (_uncommonSkillCount >= uncommonSkillMaxCount) return false;
                    if (!Helper.IsCritical(uncommonSkillChance)) return false;
                    break;
                case SkillRarity.RARE:
                    //if (_rareSkillCount >= rareSkillMaxCount) return false;
                    if (!Helper.IsCritical(rareSkillChance)) return false;
                    break;
                case SkillRarity.UNIQUE:
                    //if (_uniqueSkillCount >= uniqueSkillMaxCount) return false;
                    if (!Helper.IsCritical(uniqueSkillChance)) return false;
                    break;
            }

            return true;
        }

        int count = 0;

        while (count < 1000)
        {
            Array values = Enum.GetValues(typeof(SkillRarity));
            System.Random random = new System.Random();
            SkillRarity randomSkillRarity = (SkillRarity)values.GetValue(random.Next(values.Length));
            
            var skill = GetRandomSkill(randomSkillRarity);
            
            //print($"{skill.GetName} попытка {count}");
            
            if (skill != null && IsAvailableToAdd(skill))
            {
                _currentChoiceList.Add(skill);
                return skill;
            }

            count++;
        }

        print($"Не нашли скилл с {count} попытки");
        return lastSkill;
    }

    private Skill GetRandomSkill(SkillRarity skillRarity)
    {
        System.Random random = new System.Random();
        
        switch (skillRarity)
        {
            case SkillRarity.COMMON:
                return commonSkills.Count == 0 ? null : commonSkills[random.Next(commonSkills.Count)];
            case SkillRarity.UNCOMMON:
                return uncommonSkills.Count == 0 ? null :  uncommonSkills[random.Next(uncommonSkills.Count)];
            case SkillRarity.RARE:
                return rareSkills.Count == 0 ? null :  rareSkills[random.Next(rareSkills.Count)];
            case SkillRarity.UNIQUE:
                return uniqueSkills.Count == 0 ? null :  uniqueSkills[random.Next(uniqueSkills.Count)];
            default:
                return commonSkills.Count == 0 ? null :  commonSkills[random.Next(commonSkills.Count)];
        }
    }

    private bool IsInSkillsList(Skill skill) => _currentSkillsList.Exists(item => item.skill.GetID == skill.GetID);

    public void AddToSkillsList(Skill skill)
    {
        if (!IsInSkillsList(skill))
        {
            _currentSkillsList.Add(new SkillStruct
            {
                id = skill.GetID,
                title = skill.GetName,
                level = 1,
                skill = skill
            });
            skill.Activate();
        }
        else
        {
            var existSkillStruct = _currentSkillsList.Find(item => item.skill.GetID == skill.GetID);
            existSkillStruct.level++;
            existSkillStruct.skill.Activate();
        }
        
        if(skill is not id_1003_skillDouble_uncommon)
            SetNextSkillDouble(false);
        
        ResetChoiceList();
    }

    public void SetNextSkillDouble(bool value)
    {
        IsNextDouble = value;
    }

    [ContextMenu("Получить maxClip скилл в списке обычных")]
    private void Get1Skill()
    {
        AddToSkillsList(commonSkills[6]);
    }
    
    [ContextMenu("Получить skillDouble скилл в списке необычных")]
    private void Get2Skill()
    {
        AddToSkillsList(uncommonSkills[2]);
    }
    
    [ContextMenu("Получить health скилл в списке необычных")]
    private void Get3Skill()
    {
        AddToSkillsList(uncommonSkills[4]);
    }
}
