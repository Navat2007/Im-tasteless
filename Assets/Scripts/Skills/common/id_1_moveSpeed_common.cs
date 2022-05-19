using UnityEngine;

namespace Skills.common
{
    internal class id_1_moveSpeed_common : Skill
    {
        [SerializeField] private float bonusMoveSpeedPercent;
        
        public override void Activate()
        {
            currentLevel++;
            var percentToAdd = ControllerManager.player.MoveSpeed / 100 * bonusMoveSpeedPercent;
            ControllerManager.player.AddBonusMoveSpeed(percentToAdd);
        }
    }
}
