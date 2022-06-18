using System;
using UnityEngine;

namespace Skills.rare
{
    internal class id_3007_healOnLevel_unique : Skill
    {
        [SerializeField] private float onLevelHealPercent = 20;
        [SerializeField] private int onLevelAmmoPercent = 20;
        
        public override void Activate()
        {
            if (ControllerManager.experienceSystem != null)
            {
                ControllerManager.experienceSystem.OnLevelChange += ExperienceSystemOnLevelChange;
            }
        }

        private void OnDestroy()
        {
            if (ControllerManager.experienceSystem != null)
            {
                ControllerManager.experienceSystem.OnLevelChange -= ExperienceSystemOnLevelChange;
            }
        }

        private void ExperienceSystemOnLevelChange(int level)
        {
            ControllerManager.playerHealthSystem.AddHealthPercent(onLevelHealPercent);
            
            if(ControllerManager.weaponController.IsWeaponActive(WeaponType.SHOTGUN))
                ControllerManager.weaponController.AddAmmo(onLevelAmmoPercent, WeaponType.SHOTGUN, false, true);
            
            if(ControllerManager.weaponController.IsWeaponActive(WeaponType.RIFLE))
                ControllerManager.weaponController.AddAmmo(onLevelAmmoPercent, WeaponType.RIFLE, false, true);
            
            if(ControllerManager.weaponController.IsWeaponActive(WeaponType.GRENADE))
                ControllerManager.weaponController.AddAmmo(onLevelAmmoPercent, WeaponType.GRENADE, false, true);
        }
    }
}
