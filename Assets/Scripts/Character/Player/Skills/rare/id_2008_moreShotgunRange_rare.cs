using UnityEngine;

namespace Skills.rare
{
    internal class id_2008_moreShotgunRange_rare : Skill
    {
        [SerializeField] private int moreBulletsToShoot = 8;
        [SerializeField] private int lessAmmoInClip = 3;
        
        public override void Activate()
        {
            base.Activate();
            
            Weapon shotgun = ControllerManager.weaponController.GetWeapon(WeaponType.SHOTGUN);
            shotgun.SetProjectilePerClip(shotgun.ProjectilePerClip - lessAmmoInClip);
            shotgun.ActivateMoreProjectilePoints(moreBulletsToShoot);
        }
    }
}
