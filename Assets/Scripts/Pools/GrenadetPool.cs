using System.Collections.Generic;
using UnityEngine;

namespace Pools
{
    public class GrenadetPool : MonoBehaviour
    {
        public static GrenadetPool Instance { get; private set; }

        [Header("Prefabs")]
        [SerializeField] private Grenade grenadePrefab;
        
        [Header("Pools")]
        [SerializeField] private Transform grenadePool;

        private Queue<Grenade> _grenades = new();
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
            AddGrenade(10);
        }
        
        private void AddGrenade(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var grenade = Instantiate(grenadePrefab, grenadePool);
                grenade.gameObject.SetActive(false);
                _grenades.Enqueue(grenade);
            }
        }

        public Grenade Get()
        {
            if(_grenades.Count == 0) AddGrenade(1);
            return _grenades.Dequeue();
        }

        public void ReturnToPool(Grenade grenade)
        {
            grenade.gameObject.SetActive(false);
            _grenades.Enqueue(grenade);
        }
    }
}
