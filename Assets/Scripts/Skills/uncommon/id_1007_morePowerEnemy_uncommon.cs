using UnityEngine;

namespace Skills.uncommon
{
    internal class id_1007_morePowerEnemy_uncommon : Skill
    {
        [SerializeField] private float chance = 10f;
        
        public override void Activate()
        {
            ControllerManager.spawner.AddPowerEnemySpawnChance(ControllerManager.skillController.IsNextDouble
                ? chance * 2 : chance);
        }
    }
}
