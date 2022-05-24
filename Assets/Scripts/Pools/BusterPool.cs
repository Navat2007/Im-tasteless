using System.Collections.Generic;
using UnityEngine;

public class BusterPool : MonoBehaviour
{
    public static BusterPool Instance { get; private set; }

    [Header("Prefabs")] 
    [SerializeField] private Buster attackSpeedPrefab;
    [SerializeField] private Buster bandagePrefab;
    [SerializeField] private Buster bodyArmorPrefab;
    [SerializeField] private Buster clipPrefab;
    [SerializeField] private Buster damagePrefab;
    [SerializeField] private Buster firstAidKitPrefab;
    [SerializeField] private Buster grenadePrefab;
    [SerializeField] private Buster moveSpeedPrefab;

    [Header("Pools")] 
    [SerializeField] private Transform attackSpeedPool;
    [SerializeField] private Transform bandagePool;
    [SerializeField] private Transform bodyArmorPool;
    [SerializeField] private Transform clipPool;
    [SerializeField] private Transform damagePool;
    [SerializeField] private Transform firstAidKitPool;
    [SerializeField] private Transform grenadePool;
    [SerializeField] private Transform moveSpeedPool;

    private Queue<Buster> _attackSpeedBusters = new();
    private Queue<Buster> _bandageBusters = new();
    private Queue<Buster> _bodyArmorBusters = new();
    private Queue<Buster> _clipBusters = new();
    private Queue<Buster> _damageBusters = new();
    private Queue<Buster> _firstAidKitBusters = new();
    private Queue<Buster> _grenadeBusters = new();
    private Queue<Buster> _moveSpeedBusters = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GeneratePools();
    }

    private void GeneratePools()
    {
        AddBuster(50, BusterType.ATTACK_SPEED);
        AddBuster(50, BusterType.BANDAGE);
        AddBuster(50, BusterType.BODY_ARMOR);
        AddBuster(50, BusterType.CLIP);
        AddBuster(50, BusterType.DAMAGE);
        AddBuster(50, BusterType.FIRST_AID_KIT);
        AddBuster(50, BusterType.GRENADE);
        AddBuster(50, BusterType.MOVE_SPEED);
    }

    private void AddBuster(int count, BusterType busterType)
    {
        switch (busterType)
        {
            case BusterType.ATTACK_SPEED:
                for (int i = 0; i < count; i++)
                {
                    var buster = Instantiate(attackSpeedPrefab, attackSpeedPool);
                    buster.SetBusterType(busterType);
                    buster.gameObject.SetActive(false);
                    _attackSpeedBusters.Enqueue(buster);
                }
                break;
            case BusterType.BANDAGE:
                for (int i = 0; i < count; i++)
                {
                    var buster = Instantiate(bandagePrefab, bandagePool);
                    buster.SetBusterType(busterType);
                    buster.gameObject.SetActive(false);
                    _bandageBusters.Enqueue(buster);
                }
                break;
            case BusterType.BODY_ARMOR:
                for (int i = 0; i < count; i++)
                {
                    var buster = Instantiate(bodyArmorPrefab, bodyArmorPool);
                    buster.SetBusterType(busterType);
                    buster.gameObject.SetActive(false);
                    _bodyArmorBusters.Enqueue(buster);
                }
                break;
            case BusterType.CLIP:
                for (int i = 0; i < count; i++)
                {
                    var buster = Instantiate(clipPrefab, clipPool);
                    buster.SetBusterType(busterType);
                    buster.gameObject.SetActive(false);
                    _clipBusters.Enqueue(buster);
                }
                break;
            case BusterType.DAMAGE:
                for (int i = 0; i < count; i++)
                {
                    var buster = Instantiate(damagePrefab, damagePool);
                    buster.SetBusterType(busterType);
                    buster.gameObject.SetActive(false);
                    _damageBusters.Enqueue(buster);
                }
                break;
            case BusterType.FIRST_AID_KIT:
                for (int i = 0; i < count; i++)
                {
                    var buster = Instantiate(firstAidKitPrefab, firstAidKitPool);
                    buster.SetBusterType(busterType);
                    buster.gameObject.SetActive(false);
                    _firstAidKitBusters.Enqueue(buster);
                }
                break;
            case BusterType.GRENADE:
                for (int i = 0; i < count; i++)
                {
                    var buster = Instantiate(grenadePrefab, grenadePool);
                    buster.SetBusterType(busterType);
                    buster.gameObject.SetActive(false);
                    _grenadeBusters.Enqueue(buster);
                }
                break;
            case BusterType.MOVE_SPEED:
                for (int i = 0; i < count; i++)
                {
                    var buster = Instantiate(moveSpeedPrefab, moveSpeedPool);
                    buster.SetBusterType(busterType);
                    buster.gameObject.SetActive(false);
                    _moveSpeedBusters.Enqueue(buster);
                }
                break;
        }
    }

    public Buster Get(BusterType busterType)
    {
        switch (busterType)
        {
            case BusterType.ATTACK_SPEED:
                if (_attackSpeedBusters.Count == 0) AddBuster(1, busterType);
                return _attackSpeedBusters.Dequeue();
            case BusterType.BANDAGE:
                if (_bandageBusters.Count == 0) AddBuster(1, busterType);
                return _bandageBusters.Dequeue();
            case BusterType.BODY_ARMOR:
                if (_bodyArmorBusters.Count == 0) AddBuster(1, busterType);
                return _bodyArmorBusters.Dequeue();
            case BusterType.CLIP:
                if (_clipBusters.Count == 0) AddBuster(1, busterType);
                return _clipBusters.Dequeue();
            case BusterType.DAMAGE:
                if (_damageBusters.Count == 0) AddBuster(1, busterType);
                return _damageBusters.Dequeue();
            case BusterType.FIRST_AID_KIT:
                if (_firstAidKitBusters.Count == 0) AddBuster(1, busterType);
                return _firstAidKitBusters.Dequeue();
            case BusterType.GRENADE:
                if (_grenadeBusters.Count == 0) AddBuster(1, busterType);
                return _grenadeBusters.Dequeue();
            case BusterType.MOVE_SPEED:
                if (_moveSpeedBusters.Count == 0) AddBuster(1, busterType);
                return _moveSpeedBusters.Dequeue();
        }

        return null;
    }

    public void ReturnToPool(Buster buster)
    {
        buster.gameObject.SetActive(false);

        switch (buster.GetBusterType)
        {
            case BusterType.ATTACK_SPEED:
                _attackSpeedBusters.Enqueue(buster);
                break;
            case BusterType.BANDAGE:
                _bandageBusters.Enqueue(buster);
                break;
            case BusterType.BODY_ARMOR:
                _bodyArmorBusters.Enqueue(buster);
                break;
            case BusterType.CLIP:
                _clipBusters.Enqueue(buster);
                break;
            case BusterType.DAMAGE:
                _damageBusters.Enqueue(buster);
                break;
            case BusterType.FIRST_AID_KIT:
                _firstAidKitBusters.Enqueue(buster);
                break;
            case BusterType.GRENADE:
                _grenadeBusters.Enqueue(buster);
                break;
            case BusterType.MOVE_SPEED:
                _moveSpeedBusters.Enqueue(buster);
                break;
        }
    }
}