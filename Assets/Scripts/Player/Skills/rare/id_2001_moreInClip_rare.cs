using System;
using UnityEngine;

namespace Skills.rare
{
    internal class id_2001_moreInClip_rare : Skill
    {
        [SerializeField] private float moreAmmoInClipPercent = 100;
        
        public override void Activate()
        {
            base.Activate();

            var pistol = ControllerManager.weaponController.GetWeapon(WeaponType.PISTOL);
            var shotgun = ControllerManager.weaponController.GetWeapon(WeaponType.SHOTGUN);
            var rifle = ControllerManager.weaponController.GetWeapon(WeaponType.RIFLE);
            
            pistol.SetProjectilePerClip(pistol.ProjectilePerClip + (int)(Convert.ToDouble(pistol.ProjectilePerClip) / 100 * moreAmmoInClipPercent));
            shotgun.SetProjectilePerClip(shotgun.ProjectilePerClip + (int)(Convert.ToDouble(shotgun.ProjectilePerClip) / 100 * moreAmmoInClipPercent));
            rifle.SetProjectilePerClip(rifle.ProjectilePerClip + (int)(Convert.ToDouble(rifle.ProjectilePerClip) / 100 * moreAmmoInClipPercent));
            
            shotgun.SeMaxAmount(shotgun.MaxProjectileAmount / 2);
            rifle.SeMaxAmount(shotgun.MaxProjectileAmount / 2);
        }
    }
}
