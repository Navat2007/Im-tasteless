using System.Collections.Generic;

namespace Skills.rare
{
    internal class id_3005_unlimitedAmmo_unique : Skill
    {
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
            
            ControllerManager.weaponController.SetInfinite(weapon);
        }
    }
}