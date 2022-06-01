using UnityEngine;

namespace Skills.rare
{
    internal class id_2014_ability_Sprint_rare : Skill
    {
        [SerializeField] private Ability ability;
        
        public override void Activate()
        {
            base.Activate();
            
            ControllerManager.playerAbilityController.SetAbility(ability);
        }
    }
}
