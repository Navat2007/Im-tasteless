using UnityEngine;

namespace Skills.uncommon
{
    internal class id_1011_moreHealth_uncommon : Skill
    {
        [SerializeField] private float bonusMoveSpeedPercent = 20f;
        [SerializeField] private float damage = -10f;

        public override void Activate()
        {
            ControllerManager.weaponController.AddBonusDamagePercent(ControllerManager.skillController.IsNextDouble
                ? damage * 2 : damage);
            
            var percentToAdd = ControllerManager.player.MoveSpeed / 100 * bonusMoveSpeedPercent;
            ControllerManager.player.AddBonusMoveSpeed(ControllerManager.skillController.IsNextDouble 
                ? percentToAdd * 2 : percentToAdd);
        }
    }
}
