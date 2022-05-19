using Interface;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(WeaponController))]
[RequireComponent(typeof(BusterController))]
[RequireComponent(typeof(HealthSystem))]
public class Player : MonoBehaviour, IHealth, IDamageable
{
    [field: SerializeField] public float Health { get; set; } = 20;
    [field: SerializeField] public float MoveSpeed { get; private set; } = 5;
    [field: SerializeField] public float BonusMoveSpeed { get; private set; }
    
    private AudioListener _cameraAudioListener;
    private HealthSystem _healthSystem;

    private void Awake()
    {
        _cameraAudioListener = Camera.main.GetComponent<AudioListener>();
        _healthSystem = GetComponent<HealthSystem>();
    }

    private void OnEnable()
    {
        ControllerManager.player = this;
        ControllerManager.healthSystem = _healthSystem;
        
        _healthSystem.OnDeath += OnDeath;
        SetCameraAudioListener(false);
    }

    private void OnDisable()
    {
        ControllerManager.player = null;
        ControllerManager.healthSystem = null;
        
        _healthSystem.OnDeath -= OnDeath;
        SetCameraAudioListener(true);
    }

    private void OnDestroy()
    {
        SetCameraAudioListener(true);
    }
    
    private void OnDeath(ProjectileHitInfo obj)
    {
        gameObject.SetActive(false);
    }

    private void SetCameraAudioListener(bool value)
    {
        if(_cameraAudioListener != null)
            _cameraAudioListener.enabled = value;
    }

    public void AddBonusMoveSpeed(float value)
    {
        BonusMoveSpeed += value;
    }
}
