using UnityEngine;

namespace Skills.rare
{
    internal class id_2016_pistolFireBullets_rare : Skill
    {
        [SerializeField] private float bonusDamagePercent = -50;
        [SerializeField] private float bonusPeriodDamagePercent = 50;
        [SerializeField] private float bonusPeriodDamageTick = 0.5f;
        [SerializeField] private float bonusPeriodDamageDuration = 2;
        
        public override void Activate()
        {
            base.Activate();
            
            Weapon pistol = ControllerManager.weaponController.GetWeapon(WeaponType.PISTOL);
            
            pistol.AddDamage(pistol.Damage / 100 * bonusDamagePercent);
            pistol.AddPeriodDamage(pistol.Damage / 100 * bonusPeriodDamagePercent);
            pistol.AddPeriodDamageDuration(bonusPeriodDamageDuration);
            pistol.AddPeriodDamageTick(bonusPeriodDamageTick);
        }
    }
}
