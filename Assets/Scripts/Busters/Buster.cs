using Interface;
using UnityEngine;

public abstract class Buster : MonoBehaviour, IPickable
{
    [Header("Скорость вращения")]
    [field: SerializeField] protected float speed = 20;
    
    [Header("Колличество предметов")]
    [field: SerializeField] protected int count;
    
    protected BusterController busterController;
    protected BusterType _busterType;
    
    private void Awake()
    {
        busterController = ControllerManager.busterController;
    }
    
    void Update()
    {
        Rotate();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PickUp();
        }
    }

    public BusterType GetBusterType => _busterType;

    public void SetBusterType(BusterType busterType)
    {
        _busterType = busterType;
    }

    public void Setup(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }
    
    public abstract void PickUp();
    
    public void Rotate()
    {
        transform.Rotate(0, speed * Time.deltaTime, 0, Space.Self);
    }
}

public enum BusterType
{
    FIRST_AID_KIT,
    BANDAGE,
    CLIP,
    GRENADE,
    BODY_ARMOR,
    DAMAGE,
    ATTACK_SPEED,
    MOVE_SPEED
}
