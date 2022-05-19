using UnityEngine;

namespace Skills.common
{
    internal class id_8_takeClip_common : Skill
    {
        [SerializeField] private int bonusTakeClip;
        
        public override void Activate()
        {
            currentLevel++;
            ControllerManager.weaponController.AddBonusTakeClip(bonusTakeClip);
        }
    }
}
