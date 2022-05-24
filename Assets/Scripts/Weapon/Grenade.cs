using Interface;
using Pools;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float damage = 150f;
    [SerializeField] private float delay = 2f;
    [SerializeField] private float radius = 5f;
    [SerializeField] private float force = 20f;
    [SerializeField] private float upwardsModifier = 0;
    [SerializeField] private ForceMode forceMode;
    [SerializeField] private float stunTime = 1;
    
    [Space(10)]
    [SerializeField] private AudioClip explodeClip;
    
    [Space(10)]
    [SerializeField] private MeshRenderer grenade1MeshRenderer;
    [SerializeField] private MeshRenderer grenade2MeshRenderer;
    [SerializeField] private MeshRenderer grenade3MeshRenderer;

    private bool _isExploded;
    private float _countdown;
    private Collider _collider;
    private Rigidbody _rigidbody;
    private float _timer;
    private ParticleSystem _explosiveParticleSystem;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _countdown = delay;
    }

    private void Update()
    {
        _countdown -= Time.deltaTime;

        if (Time.time > _timer && _isExploded)
        {
            ExplosivePool.Instance.ReturnToPool(_explosiveParticleSystem);
            GrenadetPool.Instance.ReturnToPool(this);
        }

        if (_countdown <= 0 && !_isExploded)
        {
            Explode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Small trees") || collision.gameObject.layer == LayerMask.NameToLayer("Small Obstacle"))
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), _collider);
        }
        else
            Explode();
    }

    public void Setup(Vector3 position, Quaternion rotation, Vector3 forceToAdd)
    {
        _isExploded = false;
        transform.position = position;
        transform.rotation = rotation;

        _countdown = delay;

        grenade1MeshRenderer.enabled = true;
        grenade2MeshRenderer.enabled = true;
        grenade3MeshRenderer.enabled = true;
        
        _rigidbody.AddForce(forceToAdd, ForceMode.Impulse);
    }

    private void Explode()
    {
        if(_isExploded)
            return;

        _isExploded = true;
        
        void ShowEffect()
        {
            _explosiveParticleSystem = ExplosivePool.Instance.Get();
            _explosiveParticleSystem.transform.position = transform.position + new Vector3(0, 1.1f, 0);
            _explosiveParticleSystem.transform.rotation = transform.rotation;
            _explosiveParticleSystem.gameObject.SetActive(true);
            _timer = Time.time + _explosiveParticleSystem.main.startLifetime.constantMax;
        }

        void AddForce()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

            foreach (var nearbyObjects in colliders)
            {
                if (nearbyObjects.TryGetComponent(out Rigidbody rigidbody))
                {
                    if (nearbyObjects.TryGetComponent(out EnemyController controller))
                    {
                        controller.StopNavMeshAgent();
                        controller.StartNavMeshAgent(stunTime);
                    }
                    rigidbody.AddExplosionForce(force, transform.position, radius, upwardsModifier, forceMode);
                }
                
                if (nearbyObjects.GetComponent<IDamageable>() != null && nearbyObjects.TryGetComponent(out HealthSystem healthSystem))
                {
                    var heading = nearbyObjects.transform.position - transform.position;
                    var distance = heading.magnitude;
                    var direction = heading / distance;
                    
                    healthSystem.TakeDamage(new ProjectileHitInfo
                    {
                        damage = nearbyObjects.gameObject.GetComponent<Player>() != null ? damage / 2 : damage,
                        isCritical = Helper.IsCritical(0),
                        criticalBonus = 0,
                        hitPoint = nearbyObjects.transform.position,
                        hitDirection = direction
                    });
                }
            }
        }

        void ExplodeSound()
        {
            AudioManager.instance.PlaySound(explodeClip, transform.position);
        }
        
        ShowEffect();
        AddForce();
        ExplodeSound();
        
        grenade1MeshRenderer.enabled = false;
        grenade2MeshRenderer.enabled = false;
        grenade3MeshRenderer.enabled = false;
    }
}
