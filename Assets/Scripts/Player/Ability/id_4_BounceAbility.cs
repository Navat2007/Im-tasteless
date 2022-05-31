using UnityEngine;

public class id_4_BounceAbility : Ability
{
    [SerializeField] private float speed = 5;
    
    public override void Activate()
    {
        ControllerManager.playerAbilityController.ActivateBounce(speed);
    }
}