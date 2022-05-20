using UnityEngine;

namespace Skills.uncommon
{
    internal class id_1004_penetrate_uncommon : Skill
    {
        [SerializeField] private int penetrateCount = 1;
        
        public override void Activate()
        {
            ControllerManager.weaponController.AddPenetrateCount(ControllerManager.skillController.IsNextDouble 
                ? penetrateCount * 2 : penetrateCount);
        }
    }
}
