using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(WeaponController))]
[RequireComponent(typeof(BusterController))]
[RequireComponent(typeof(HealthSystem))]
public class Player : Character.Character
{
    [field: SerializeField] public override float Health { get; set; } = 20;
    [field: SerializeField] public float MoveSpeed { get; private set; } = 5;
    [field: SerializeField] public float BonusMoveSpeed { get; private set; }
    [field: SerializeField] public float ExplosiveResist { get; private set; } = 50;
    [field: SerializeField] public float HealOnTakeAmmoPercent { get; private set; }
    [field: SerializeField] public Renderer Renderer { get; private set; }
    [field: SerializeField] public List<Transform> EnemyTeleportPoints { get; private set; }
    
    private AudioListener _cameraAudioListener;
    private HealthSystem _healthSystem;

    private void Awake()
    {
        _cameraAudioListener = Camera.main.GetComponent<AudioListener>();
        _healthSystem = GetComponent<HealthSystem>();
        
        _healthSystem.SetRender(Renderer);
        
        EventBus.PlayerEvents.OnRevive += OnRevive;
    }

    private void OnEnable()
    {
        ControllerManager.player = this;
        ControllerManager.playerHealthSystem = _healthSystem;

        _healthSystem.OnDeath += OnDeath;
        SetCameraAudioListener(false);
    }

    private void Start()
    {
        _healthSystem.Init(Health);
    }

    private void OnDisable()
    {
        ControllerManager.player = null;
        ControllerManager.playerHealthSystem = null;
        
        _healthSystem.OnDeath -= OnDeath;
        SetCameraAudioListener(true);
    }

    private void OnDestroy()
    {
        SetCameraAudioListener(true);
        
        EventBus.PlayerEvents.OnRevive -= OnRevive;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 30);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 50);
    }

    private void OnDeath(GameObject owner, ProjectileHitInfo obj)
    {
        gameObject.SetActive(false);
    }
    
    private void OnRevive()
    {
        gameObject.SetActive(true);
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
    
    public void AddExplosiveResist(float value)
    {
        ExplosiveResist += value;
    }
    
    public void SetExplosiveResist(float value)
    {
        ExplosiveResist = value;
    }
    
    public void AddHealOnTakeAmmoPercent(float value)
    {
        HealOnTakeAmmoPercent = value;
    }

    public List<Transform> GetTeleportPoints => EnemyTeleportPoints;
}
