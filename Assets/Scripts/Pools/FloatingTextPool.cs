using System.Collections.Generic;
using UnityEngine;

namespace Pools
{
    public class FloatingTextPool : MonoBehaviour
    {
        public static FloatingTextPool Instance { get; private set; }

        [Header("Prefabs")]
        [SerializeField] private FloatingText floatingTextPrefab;
        
        [Header("Pools")]
        [SerializeField] private Transform floatingTextPool;

        private Queue<FloatingText> _floatingTexts = new();
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
            AddFloatingText(50);
        }
        
        private void AddFloatingText(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var floatingText = Instantiate(floatingTextPrefab, floatingTextPool);
                floatingText.gameObject.SetActive(false);
                _floatingTexts.Enqueue(floatingText);
            }
        }

        public FloatingText Get()
        {
            if(_floatingTexts.Count == 0) AddFloatingText(1);
            return _floatingTexts.Dequeue();
        }

        public void ReturnToPool(FloatingText floatingText)
        {
            floatingText.gameObject.SetActive(false);
            _floatingTexts.Enqueue(floatingText);
        }
    }
}
