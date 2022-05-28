using UnityEngine;

namespace Skills.uncommon
{
    internal class id_1006_instantKill_uncommon : Skill
    {
        [SerializeField] private float killChance = 3f;
        
        public override void Activate()
        {
            ControllerManager.weaponController.AddKillChance(ControllerManager.skillController.IsNextDouble 
                ? killChance * 2 : killChance);
        }
    }
}
