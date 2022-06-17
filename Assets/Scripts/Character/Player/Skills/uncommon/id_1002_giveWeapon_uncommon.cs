using System.Collections.Generic;
using UnityEngine;

namespace Skills.uncommon
{
    internal class id_1002_giveWeapon_uncommon : Skill
    {
        [SerializeField] private int addPercent = 50;
        
        public override void Activate()
        {
            List<WeaponType> activeWeapon = new()
            {
                WeaponType.SHOTGUN,
                WeaponType.RIFLE,
                WeaponType.GRENADE
            };
            
            System.Random random = new System.Random();
            var weapon = activeWeapon[random.Next(activeWeapon.Count)];
            
            ControllerManager.weaponController.AddAmmo(ControllerManager.skillController.IsNextDouble 
                ? addPercent * 2 : addPercent, weapon, true, true);
        }
    }
}
