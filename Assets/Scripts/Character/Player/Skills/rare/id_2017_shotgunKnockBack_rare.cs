using UnityEngine;

namespace Skills.rare
{
    internal class id_2017_shotgunKnockBack_rare : Skill
    {
        [SerializeField] private int moreBulletsToShoot = 2;
        
        public override void Activate()
        {
            base.Activate();
            
            Weapon shotgun = ControllerManager.weaponController.GetWeapon(WeaponType.SHOTGUN);
            shotgun.ActivateMoreProjectilePoints(moreBulletsToShoot);
            shotgun.SetKnockBack(true);
        }
    }
}
