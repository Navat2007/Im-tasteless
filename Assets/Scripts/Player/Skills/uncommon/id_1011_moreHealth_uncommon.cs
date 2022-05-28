using UnityEngine;

namespace Skills.uncommon
{
    internal class id_1011_moreHealth_uncommon : Skill
    {
        [SerializeField] private float maxHealth = 20f;
        [SerializeField] private float damage = -10f;

        public override void Activate()
        {
            ControllerManager.weaponController.AddBonusDamagePercent(ControllerManager.skillController.IsNextDouble
                ? damage * 2 : damage);
            ControllerManager.healthSystem.AddMaxHealthPercent(ControllerManager.skillController.IsNextDouble
                ? maxHealth * 2 : maxHealth);
        }
    }
}
