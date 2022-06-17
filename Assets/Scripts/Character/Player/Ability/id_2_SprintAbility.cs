using UnityEngine;

public class id_2_SprintAbility : Ability
{
    [SerializeField] private float movePercent = 50;
    
    public override void Activate()
    {
        ControllerManager.playerAbilityController.ActivateSprint(duration, movePercent);
    }
}