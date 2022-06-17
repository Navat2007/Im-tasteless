using UnityEngine;

namespace Skills.uncommon
{
    internal class id_1010_moreAttackSpeed_uncommon : Skill
    {
        [SerializeField] private float attackSpeed = 20f;
        [SerializeField] private float reloadSpeed = -10f;
        
        
        public override void Activate()
        {
            ControllerManager.weaponController.AddBonusReloadSpeedPercent(ControllerManager.skillController.IsNextDouble
                ? reloadSpeed * 2 : reloadSpeed);
            ControllerManager.weaponController.AddBonusAttackSpeedPercent(ControllerManager.skillController.IsNextDouble
                ? attackSpeed * 2 : attackSpeed);
        }
    }
}
