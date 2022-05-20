using UnityEngine;

namespace Skills.uncommon
{
    internal class id_1005_healthRegen_uncommon : Skill
    {
        [SerializeField] private float healthPercent = 0.5f;
        
        public override void Activate()
        {
            var valueToAdd = ControllerManager.healthSystem.MaxHealth / 100 * healthPercent;
            
            ControllerManager.healthSystem.AddHealthInSecond(ControllerManager.skillController.IsNextDouble 
                ? valueToAdd * 2 : valueToAdd);
        }
    }
}
