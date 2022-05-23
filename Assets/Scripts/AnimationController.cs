using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationController : MonoBehaviour
{
    [SerializeField] private RuntimeAnimatorController[] animatorControllers;
    
    private Animator _animator;
    private AnimationState _currentState;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        if (animatorControllers.Length > 0)
        {
            System.Random random = new System.Random();
            _animator.runtimeAnimatorController = animatorControllers[random.Next(animatorControllers.Length)];
        }
    }

    public void SetState(AnimationState state)
    {
        if(_currentState == state || _currentState == AnimationState.DIE) return;
        
        switch (state)
        {
            case AnimationState.IDLE:
                _animator.SetFloat("Speed", 0); 
                break;
            case AnimationState.WALK:
                _animator.SetFloat("Speed", 0.4f);  
                break;
            case AnimationState.RUN:
                _animator.SetFloat("Speed", 1f); 
                break;
            case AnimationState.HIT:
                _animator.SetTrigger("Hit"); 
                break;
            case AnimationState.ATTACK:
                System.Random random = new System.Random();
                switch (random.Next(1, 3))
                {
                    case 1:
                        _animator.SetTrigger("Attack1"); 
                        break;
                    case 2:
                        _animator.SetTrigger("Attack2"); 
                        break;
                    case 3:
                        _animator.SetTrigger("Attack3"); 
                        break;
                }
                break;
            case AnimationState.DIE:
                _animator.SetBool("Death", true); 
                break;
        }

        _currentState = state;
    }
}

public enum AnimationState
{
    IDLE,
    WALK,
    RUN,
    HIT,
    ATTACK,
    DIE
}
