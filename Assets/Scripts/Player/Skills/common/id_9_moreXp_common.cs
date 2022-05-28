using UnityEngine;

namespace Skills.common
{
    internal class id_9_moreXp_common : Skill
    {
        [SerializeField] private float bonusXpPercent;
        [SerializeField] private int bonusMaxLevel;
        
        public override void Activate()
        {
            ControllerManager.experienceSystem.AddBonusXpPercent(ControllerManager.skillController.IsNextDouble 
                ? bonusXpPercent * 2 : bonusXpPercent);
            ControllerManager.experienceSystem.AddMaxLevel(ControllerManager.skillController.IsNextDouble 
                ? bonusMaxLevel * 2 : bonusMaxLevel);
        }
    }
}
