using UnityEngine;

namespace Skills.common
{
    internal class id_4_reloadSpeed_common : Skill
    {
        [SerializeField] private float bonusReloadSpeed;
        
        public override void Activate()
        {
            ControllerManager.weaponController.AddBonusReloadSpeedPercent(ControllerManager.skillController.IsNextDouble 
                ? bonusReloadSpeed * 2 : bonusReloadSpeed);
        }
    }
}
