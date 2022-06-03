using UnityEngine;

namespace Skills.uncommon
{
    internal class id_1012_moreDamage_uncommon : Skill
    {
        [SerializeField] private float damage = 20f;
        [SerializeField] private float bonusMoveSpeedPercent = -10f;

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
