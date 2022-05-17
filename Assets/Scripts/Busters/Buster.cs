using Interface;
using UnityEngine;

public abstract class Buster : MonoBehaviour, IPickable
{
    [Header("Скорость вращения")]
    [field: SerializeField] protected float speed = 20;
    
    [Header("Колличество предметов")]
    [field: SerializeField] protected int count;
    
    protected BusterController busterController;
    
    private void Awake()
    {
        busterController = GameObject.FindGameObjectWithTag("Player").GetComponent<BusterController>();
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
