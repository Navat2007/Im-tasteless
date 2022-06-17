using UnityEngine;

namespace Skills.common
{
    internal class id_2_weaponDamage_common : Skill
    {
        [SerializeField] private float bonusWeaponDamagePercent = 10;
        
        public override void Activate()
        {
            ControllerManager.weaponController.AddBonusDamagePercent(ControllerManager.skillController.IsNextDouble 
                ? bonusWeaponDamagePercent * 2 : bonusWeaponDamagePercent);
        }
    }
}
