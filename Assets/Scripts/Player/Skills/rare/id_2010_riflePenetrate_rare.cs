using UnityEngine;

namespace Skills.rare
{
    internal class id_2010_riflePenetrate_rare : Skill
    {
        [SerializeField] private int penetrateCount = 1;
        
        public override void Activate()
        {
            base.Activate();
            
            Weapon rifle = ControllerManager.weaponController.GetWeapon(WeaponType.RIFLE);
            rifle.AddPenetrateCount(penetrateCount);
        }
    }
}
