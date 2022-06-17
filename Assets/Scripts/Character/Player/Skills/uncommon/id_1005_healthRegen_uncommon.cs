using UnityEngine;

namespace Skills.uncommon
{
    internal class id_1005_healthRegen_uncommon : Skill
    {
        [SerializeField] private float healthPercent = 0.5f;
        
        public override void Activate()
        {
            var valueToAdd = ControllerManager.playerHealthSystem.MaxHealth / 100 * healthPercent;
            
            ControllerManager.playerHealthSystem.AddHealthInSecond(ControllerManager.skillController.IsNextDouble 
                ? valueToAdd * 2 : valueToAdd);
        }
    }
}
