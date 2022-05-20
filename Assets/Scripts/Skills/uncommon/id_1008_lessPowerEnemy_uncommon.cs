using UnityEngine;

namespace Skills.uncommon
{
    internal class id_1008_lessPowerEnemy_uncommon : Skill
    {
        [SerializeField] private float chance = -10f;
        
        public override void Activate()
        {
            ControllerManager.enemySpawner.AddPowerEnemySpawnChance(ControllerManager.skillController.IsNextDouble
                ? chance * 2 : chance);
        }
    }
}
