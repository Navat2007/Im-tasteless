using UnityEngine;

namespace Skills.common
{
    internal class id_6_criticalBonus_common : Skill
    {
        [SerializeField] private float bonusCriticalBonus;
        
        public override void Activate()
        {
            ControllerManager.weaponController.AddBonusCriticalPower(ControllerManager.skillController.IsNextDouble 
                ? bonusCriticalBonus * 2 : bonusCriticalBonus);
        }
    }
}
