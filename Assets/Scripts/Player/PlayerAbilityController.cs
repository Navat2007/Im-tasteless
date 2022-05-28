using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilityController : MonoBehaviour
{
    public event Action<float, Ability> OnAbilityUse;
    public event Action<Ability> OnAbilitySet;
    
    [SerializeField] private Ability currentAbility;

    private float _nextTimeAbilityUse;

    private void Awake()
    {
        ControllerManager.playerAbilityController = this;
    }

    private void Start()
    {
        ControllerManager.playerInput.actions["Ability"].performed += OnAbilityInputUse;
    }

    private void OnDisable()
    {
        ControllerManager.playerInput.actions["Ability"].performed -= OnAbilityInputUse;
        currentAbility = null;
        ControllerManager.playerAbilityController = null;
    }
    
    private void OnAbilityInputUse(InputAction.CallbackContext obj)
    {
        if(currentAbility == null || Time.time < _nextTimeAbilityUse) return;
        
        _nextTimeAbilityUse = Time.time + currentAbility.GetReuseTimer;
        currentAbility.Activate();
        OnAbilityUse?.Invoke(currentAbility.GetReuseTimer, currentAbility);
    }

    public void SetAbility(Ability ability)
    {
        currentAbility = ability;
        OnAbilitySet?.Invoke(currentAbility);
    }

    public void ActivateSprint(float duration, float movePercent)
    {
        IEnumerator Sprint()
        {
            var speedToAdd = ControllerManager.player.MoveSpeed / 100 * movePercent;
            ControllerManager.player.AddBonusMoveSpeed(speedToAdd);
            yield return new WaitForSeconds(duration);
            ControllerManager.player.AddBonusMoveSpeed(-speedToAdd);
        }

        StartCoroutine(Sprint());
    }
}
