using UnityEngine;

namespace Skills.rare
{
    internal class id_3004_below25_unique : Skill
    {
        [SerializeField] private float bonusMoveSpeedPercent = 25;
        [SerializeField] private float bonusAttackSpeedPercent = 50;
        [SerializeField] private float bonusReloadSpeedPercent = 50;
        
        private bool _isBonusAdded;
        private float _speedToAdd;
        
        public override void Activate()
        {
            if (ControllerManager.playerHealthSystem != null)
            {
                _speedToAdd = ControllerManager.player.MoveSpeed / 100 * bonusMoveSpeedPercent;
                CheckCondition(ControllerManager.playerHealthSystem.GetPercentCurrentMax());
                ControllerManager.playerHealthSystem.OnHealthChange += OnHealthChange;
            }
        }
        
        private void OnHealthChange(float obj)
        {
            CheckCondition(ControllerManager.playerHealthSystem.GetPercentCurrentMax());
        }
        
        private void CheckCondition(float percent)
        {
            if (percent < 25 && !_isBonusAdded)
            {
                _isBonusAdded = true;
                ControllerManager.player.AddBonusMoveSpeed(_speedToAdd);
                ControllerManager.weaponController.AddBonusAttackSpeedPercent(bonusAttackSpeedPercent);
                ControllerManager.weaponController.AddBonusReloadSpeedPercent(bonusReloadSpeedPercent);
            }
            else if(percent >= 25 && _isBonusAdded)
            {
                _isBonusAdded = false;
                ControllerManager.player.AddBonusMoveSpeed(-_speedToAdd);
                ControllerManager.weaponController.AddBonusAttackSpeedPercent(-bonusAttackSpeedPercent);
                ControllerManager.weaponController.AddBonusReloadSpeedPercent(-bonusReloadSpeedPercent);
            }
        }
    }
}
