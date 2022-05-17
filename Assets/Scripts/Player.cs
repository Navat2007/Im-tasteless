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

    private void Awake()
    {
        _cameraAudioListener = Camera.main.GetComponent<AudioListener>();
    }

    private void OnEnable()
    {
        SetCameraAudioListener(false);
    }

    private void OnDisable()
    {
        //print("Player disable");
        SetCameraAudioListener(true);
    }

    private void OnDestroy()
    {
        //print("Player destroy");
        SetCameraAudioListener(true);
    }

    private void SetCameraAudioListener(bool value)
    {
        if(_cameraAudioListener != null)
            _cameraAudioListener.enabled = value;
    }

    public void SetBonusMoveSpeed(float value)
    {
        BonusMoveSpeed += value;
    }
}
