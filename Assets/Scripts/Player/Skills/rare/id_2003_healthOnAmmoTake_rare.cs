using UnityEngine;

namespace Skills.rare
{
    internal class id_2003_healthOnAmmoTake_rare : Skill
    {
        [SerializeField] private float healPercent = 10;
        
        public override void Activate()
        {
            ControllerManager.player.AddHealOnTakeAmmoPercent(healPercent);
        }
    }
}
