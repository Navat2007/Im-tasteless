using System.Collections.Generic;

namespace Skills.rare
{
    internal class id_3009_skillsRandomWeapon_unique : Skill
    {
        public override void Activate()
        {
            List<WeaponType> activeWeapon = new()
            {
                WeaponType.SHOTGUN,
                WeaponType.RIFLE,
            };
            
            var shotgun = ControllerManager.weaponController.GetWeapon(WeaponType.SHOTGUN);
            var rifle = ControllerManager.weaponController.GetWeapon(WeaponType.RIFLE);
            
            System.Random random = new System.Random();
            var weapon = activeWeapon[random.Next(activeWeapon.Count)];
            
            switch (weapon)
            {
                case WeaponType.SHOTGUN:
                    var shotgunSkill1 = ControllerManager.skillController.GetSkillByID(2008);
                    var shotgunSkill2 = ControllerManager.skillController.GetSkillByID(2017);
                    ControllerManager.skillController.AddToSkillsList(shotgunSkill1.skill);
                    ControllerManager.skillController.AddToSkillsList(shotgunSkill2.skill);
                    
                    ControllerManager.skillController.AddToExceptedList(2009);
                    ControllerManager.skillController.AddToExceptedList(2010);
                    
                    rifle.SetDisabled(true);
                    GameUI.instance.SetSlot(3, false);
                    
                    break;
                case WeaponType.RIFLE:
                    var rifleSkill1 = ControllerManager.skillController.GetSkillByID(2009);
                    var rifleSkill2 = ControllerManager.skillController.GetSkillByID(2010);
                    ControllerManager.skillController.AddToSkillsList(rifleSkill1.skill);
                    ControllerManager.skillController.AddToSkillsList(rifleSkill2.skill);
                    
                    ControllerManager.skillController.AddToExceptedList(2008);
                    ControllerManager.skillController.AddToExceptedList(2017);
                    
                    shotgun.SetDisabled(true);
                    GameUI.instance.SetSlot(2, false);
                    
                    break;
            }
            
            ControllerManager.weaponController.EquipWeapon(WeaponType.PISTOL);
        }
    }
}
