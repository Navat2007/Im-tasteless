using UnityEngine;

namespace Skills.rare
{
    internal class id_3001_moreHeal_unique : Skill
    {
        [SerializeField] private float bonusHealPercent = 100;
        
        public override void Activate()
        {
            ControllerManager.playerHealthSystem.AddBonusHealPercent(bonusHealPercent);
        }
    }
}
