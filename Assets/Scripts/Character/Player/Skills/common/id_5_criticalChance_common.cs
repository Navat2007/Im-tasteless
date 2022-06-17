using UnityEngine;

namespace Skills.common
{
    internal class id_5_criticalChance_common : Skill
    {
        [SerializeField] private float bonusCriticalChance;
        
        public override void Activate()
        {
            ControllerManager.weaponController.AddBonusCriticalChance(ControllerManager.skillController.IsNextDouble 
                ? bonusCriticalChance * 2 : bonusCriticalChance);
        }
    }
}
