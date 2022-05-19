using UnityEngine;

namespace Skills.common
{
    internal class id_2_weaponDamage_common : Skill
    {
        [SerializeField] private float bonusWeaponDamagePercent;
        
        public override void Activate()
        {
            currentLevel++;
            ControllerManager.weaponController.AddBonusDamagePercent(bonusWeaponDamagePercent);
        }
    }
}
