using UnityEngine;

namespace Skills.rare
{
    internal class id_2007_moreBusters_rare : Skill
    {
        [SerializeField] private float percentChance = 50;
        
        public override void Activate()
        {
            ControllerManager.enemySpawner.AddBonusBusterChance(percentChance);
        }
    }
}
