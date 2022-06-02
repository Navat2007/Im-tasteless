using UnityEngine;

public class id_3_DashAbility : Ability
{
    [SerializeField] private float speed = 50;
    
    public override void Activate()
    {
        ControllerManager.playerAbilityController.ActivateDash(speed);
    }
}