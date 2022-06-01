using UnityEngine;

namespace Skills.rare
{
    internal class id_2009_rifleKillChance_rare : Skill
    {
        [SerializeField] private int killChance = 5;
        
        public override void Activate()
        {
            base.Activate();
            
            Weapon rifle = ControllerManager.weaponController.GetWeapon(WeaponType.RIFLE);
            rifle.AddKillChance(killChance);
        }
    }
}
