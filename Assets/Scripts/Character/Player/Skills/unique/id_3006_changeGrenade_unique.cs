using UnityEngine;

namespace Skills.rare
{
    internal class id_3006_changeGrenade_unique : Skill
    {
        [SerializeField] private float flaskReuseTime = 10;
        [SerializeField] private float flaskHealthPercent = 25;
        [SerializeField] private float flaskBusterPercent = 25;
        [SerializeField] private float flaskBusterDuration = 10;
        
        public override void Activate()
        {
            ControllerManager.weaponController.SetFlaskParameters(true, flaskReuseTime, flaskHealthPercent, flaskBusterPercent, flaskBusterDuration);
        }
    }
}
