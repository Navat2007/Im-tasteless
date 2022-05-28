using UnityEngine;

namespace Skills.common
{
    internal class id_7_maxClip_common : Skill
    {
        [SerializeField] private int bonusMaxClip;
        
        public override void Activate()
        {
            ControllerManager.weaponController.AddBonusMaxClip(ControllerManager.skillController.IsNextDouble 
                ? bonusMaxClip * 2 : bonusMaxClip);
        }
    }
}
