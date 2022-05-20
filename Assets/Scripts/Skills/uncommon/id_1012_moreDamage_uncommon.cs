using UnityEngine;

namespace Skills.uncommon
{
    internal class id_1012_moreDamage_uncommon : Skill
    {
        [SerializeField] private float damage = 20f;
        [SerializeField] private float maxHealth = -10f;

        public override void Activate()
        {
            ControllerManager.weaponController.AddBonusDamagePercent(ControllerManager.skillController.IsNextDouble
                ? damage * 2 : damage);
            ControllerManager.healthSystem.AddMaxHealthPercent(ControllerManager.skillController.IsNextDouble
                ? maxHealth * 2 : maxHealth);
        }
    }
}
