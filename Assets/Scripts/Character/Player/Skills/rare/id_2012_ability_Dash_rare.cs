using UnityEngine;

namespace Skills.rare
{
    internal class id_2012_ability_Dash_rare : Skill
    {
        [SerializeField] private Ability ability;
        
        public override void Activate()
        {
            base.Activate();
            
            ControllerManager.playerAbilityController.SetAbility(ability);
        }
    }
}
