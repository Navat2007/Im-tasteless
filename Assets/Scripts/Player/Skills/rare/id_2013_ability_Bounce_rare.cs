using UnityEngine;

namespace Skills.rare
{
    internal class id_2013_ability_Bounce_rare : Skill
    {
        [SerializeField] private Ability ability;
        
        public override void Activate()
        {
            ControllerManager.playerAbilityController.SetAbility(ability);
        }
    }
}
