using UnityEngine;

namespace Skills.common
{
    internal class id_9_moreXp_common : Skill
    {
        [SerializeField] private float bonusXpPercent;
        [SerializeField] private int bonusMaxLevel;
        
        public override void Activate()
        {
            currentLevel++;
            ControllerManager.experienceSystem.AddBonusXpPercent(bonusXpPercent);
            ControllerManager.experienceSystem.AddMaxLevel(bonusMaxLevel);
        }
    }
}
