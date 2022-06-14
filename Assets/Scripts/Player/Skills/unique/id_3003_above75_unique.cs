using UnityEngine;

namespace Skills.rare
{
    internal class id_3003_above75_unique : Skill
    {
        [SerializeField] private float bonusCriticalChance = 25;
        [SerializeField] private float bonusCriticalPower = 50;

        private bool _isBonusAdded;
        
        public override void Activate()
        {
            if (ControllerManager.playerHealthSystem != null)
            {
                CheckCondition(ControllerManager.playerHealthSystem.GetPercentCurrentMax());
                ControllerManager.playerHealthSystem.OnHealthChange += OnHealthChange;
            }
        }

        private void OnHealthChange(float currentHealth)
        {
            CheckCondition(ControllerManager.playerHealthSystem.GetPercentCurrentMax());
        }

        private void CheckCondition(float percent)
        {
            if (percent >= 75 && !_isBonusAdded)
            {
                _isBonusAdded = true;
                ControllerManager.weaponController.AddBonusCriticalChance(bonusCriticalChance);
                ControllerManager.weaponController.AddBonusCriticalPower(bonusCriticalPower);
            }
            else if(percent < 75 && _isBonusAdded)
            {
                _isBonusAdded = false;
                ControllerManager.weaponController.AddBonusCriticalChance(-bonusCriticalChance);
                ControllerManager.weaponController.AddBonusCriticalPower(-bonusCriticalPower);
            }
        }
    }
}
