using UnityEngine;

namespace Skills.uncommon
{
    internal class id_1009_moreReloadSpeed_uncommon : Skill
    {
        [SerializeField] private float reloadSpeed = 20f;
        [SerializeField] private float attackSpeed = -10f;
        
        public override void Activate()
        {
            ControllerManager.weaponController.AddBonusReloadSpeedPercent(ControllerManager.skillController.IsNextDouble
                ? reloadSpeed * 2 : reloadSpeed);
            ControllerManager.weaponController.AddBonusAttackSpeedPercent(ControllerManager.skillController.IsNextDouble
                ? attackSpeed * 2 : attackSpeed);
        }
    }
}
