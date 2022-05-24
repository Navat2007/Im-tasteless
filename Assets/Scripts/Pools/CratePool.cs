using System.Collections.Generic;
using UnityEngine;

namespace Pools
{
    public class CratePool : MonoBehaviour
    {
        public static CratePool Instance { get; private set; }

        [Header("Prefabs")]
        [SerializeField] private Crate cratePrefab;
        
        [Header("Pools")]
        [SerializeField] private Transform cratePool;

        private Queue<Crate> _crates = new();
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
            AddCrate(25);
        }
        
        private void AddCrate(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var crate = Instantiate(cratePrefab, cratePool);
                crate.gameObject.SetActive(false);
                _crates.Enqueue(crate);
            }
        }

        public Crate Get()
        {
            if(_crates.Count == 0) AddCrate(1);
            return _crates.Dequeue();
        }

        public void ReturnToPool(Crate crate)
        {
            crate.gameObject.SetActive(false);
            _crates.Enqueue(crate);
        }
    }
}
