using UnityEngine;

namespace Skills.rare
{
    internal class id_2006_noExploseDamage_rare : Skill
    {
        [SerializeField] private float explosiveResistPercent = 100;
        
        public override void Activate()
        {
            ControllerManager.player.SetExplosiveResist(explosiveResistPercent);
        }
    }
}
