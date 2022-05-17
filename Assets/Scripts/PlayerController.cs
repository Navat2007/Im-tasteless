using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public event Action<Vector3> OnCrosshairMove; 

    [SerializeField] private Transform crosshair;
    
    private Player _player;
    private WeaponController _weaponController;
    private PlayerInput _playerInput;
    private Rigidbody _rigidbody;
    private Animator _animator;
    private Vector3 _moveVelocity;
    private Camera _camera;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _playerInput = GetComponent<PlayerInput>();
        _weaponController = GetComponent<WeaponController>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _camera = Camera.main;
    }

    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        if (GameManager.GetGameState() == GameManager.GameState.PLAY)
        {
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane groundPlane = new Plane(Vector3.up, Vector3.up * _weaponController.GetWeaponHold.position.y);

            if (groundPlane.Raycast(ray, out var rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                //print(point);
                //Debug.DrawRay(ray.origin, point, Color.black);
                crosshair.position = point;
                LookAt(point);
                
                OnCrosshairMove?.Invoke(point);
            
                if((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1.5f)
                    _weaponController.Aim(point);
            }
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + _moveVelocity * Time.fixedDeltaTime);
        _animator.SetFloat("Forward", _moveVelocity.z);
        _animator.SetFloat("Turn", _moveVelocity.x);
    }

    private void OnEnable()
    {
        _playerInput.actions["Move"].performed += OnMove;
        _playerInput.actions["Move"].canceled += OnMove;
        _playerInput.actions["Escape"].performed += OnEscape;
    }

    private void OnDisable()
    {
        _playerInput.actions["Move"].performed -= OnMove;
        _playerInput.actions["Move"].canceled -= OnMove;
        _playerInput.actions["Escape"].performed -= OnEscape;
    }
    
    private void OnMove (InputAction.CallbackContext context)
    {
        var inputValue = context.ReadValue<Vector2>();
        _moveVelocity = new Vector3(inputValue.x, 0, inputValue.y).normalized * (_player.MoveSpeed + _player.BonusMoveSpeed);
    }

    private void LookAt(Vector3 point)
    {
        Vector3 heightCorrectedPoint = new Vector3(point.x, transform.position.y, point.z);
        transform.LookAt(heightCorrectedPoint);
    }
    
    private void OnEscape(InputAction.CallbackContext context)
    {
        GameUI.instance.ClosePanel();
    }
    
    
}
