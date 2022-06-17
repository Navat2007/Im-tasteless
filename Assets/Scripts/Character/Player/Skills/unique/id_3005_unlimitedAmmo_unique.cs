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
            };
            
            var pistol = ControllerManager.weaponController.GetWeapon(WeaponType.PISTOL);
            var shotgun = ControllerManager.weaponController.GetWeapon(WeaponType.SHOTGUN);
            var rifle = ControllerManager.weaponController.GetWeapon(WeaponType.RIFLE);
            
            System.Random random = new System.Random();
            var weapon = activeWeapon[random.Next(activeWeapon.Count)];
            
            ControllerManager.weaponController.SetInfinite(weapon);
            
            switch (weapon)
            {
                case WeaponType.SHOTGUN:
                    rifle.SeMaxAmount(rifle.MaxProjectileAmount - (rifle.MaxProjectileAmount / 100 * 10));
                    break;
                case WeaponType.RIFLE:
                    shotgun.SeMaxAmount(shotgun.MaxProjectileAmount - (shotgun.MaxProjectileAmount / 100 * 10));
                    break;
            }
        }
    }
}