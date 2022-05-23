using System.Collections.Generic;
using UnityEngine;

namespace Pools
{
    public class BulletPool : MonoBehaviour
    {
        public static BulletPool Instance { get; private set; }

        [Header("Prefabs")]
        [SerializeField] private Projectile projectilePrefab;
        
        [Header("Pools")]
        [SerializeField] private Transform projectilePool;

        private Queue<Projectile> _projectiles = new();
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
            AddProjectiles(100, ProjectileType.BULLET);
        }
        
        private void AddProjectiles(int count, ProjectileType projectileType)
        {
            switch (projectileType)
            {
                case ProjectileType.BULLET:
                    for (int i = 0; i < count; i++)
                    {
                        var pistolProjectile = Instantiate(projectilePrefab, projectilePool);
                        pistolProjectile.SetProjectileType(projectileType);
                        pistolProjectile.gameObject.SetActive(false);
                        _projectiles.Enqueue(pistolProjectile);
                    }
                    break;
            }
        }

        public Projectile Get(ProjectileType projectileType)
        {
            switch (projectileType)
            {
                case ProjectileType.BULLET:
                    if(_projectiles.Count == 0) AddProjectiles(1, projectileType);
                    return _projectiles.Dequeue();
            }

            return null;
        }

        public void ReturnToPool(Projectile projectile)
        {
            projectile.gameObject.SetActive(false);

            switch (projectile.GetProjectileType)
            {
                case ProjectileType.BULLET:
                    _projectiles.Enqueue(projectile);
                    break;
            }
        }
    }
}
