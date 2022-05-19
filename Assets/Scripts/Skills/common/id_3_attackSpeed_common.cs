using UnityEngine;

namespace Skills.common
{
    internal class id_3_attackSpeed_common : Skill
    {
        [SerializeField] private float bonusAttackSpeed;
        
        public override void Activate()
        {
            currentLevel++;
            ControllerManager.weaponController.AddBonusAttackSpeedPercent(bonusAttackSpeed);
        }
    }
}
