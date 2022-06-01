using UnityEngine;

namespace Skills.rare
{
    internal class id_2011_pistolPower_rare : Skill
    {
        [SerializeField] private AudioClip audioClip;
        [Space(10)]
        [SerializeField] private int bonusAmmoInClip = 1;
        [SerializeField] private float bonusDamagePercent = 15;
        [SerializeField] private float bonusCriticalChancePercent = 10;
        [SerializeField] private float bonusCriticalBonusPercent = 10;
        [SerializeField] private float bonusAttackSpeed = -5;
        [SerializeField] private float bonusReloadSpeed = -5;
        
        public override void Activate()
        {
            base.Activate();
            
            Weapon pistol = ControllerManager.weaponController.GetWeapon(WeaponType.PISTOL);

            pistol.AddDamage(pistol.Damage / 100 * bonusDamagePercent);
            pistol.SetCriticalChance(pistol.CriticalChance + bonusCriticalChancePercent);
            pistol.SetCriticalBonus(pistol.CriticalBonus + + (pistol.CriticalBonus / 100 * bonusCriticalBonusPercent));
            pistol.SetProjectilePerClip(pistol.ProjectilePerClip + bonusAmmoInClip);
            pistol.SetAttackSpeed(pistol.MsBetweenShots - (pistol.MsBetweenShots / 100 * bonusAttackSpeed));
            pistol.SetReloadSpeed(pistol.ReloadSpeed - (pistol.ReloadSpeed / 100 * bonusReloadSpeed));
            
            pistol.SetAudioClip(audioClip);
        }
    }
}
