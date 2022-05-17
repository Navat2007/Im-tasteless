using Interface;
using UnityEngine;

public abstract class PickableWeapon : MonoBehaviour, IPickable
{
    [Header("Скорость вращения")]
    [field: SerializeField] protected float speed = 20;
    
    [Header("Колличество предметов")]
    [field: SerializeField] protected int count;
    
    protected WeaponController weaponController;
    
    private void Awake()
    {
        weaponController = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponController>();
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
