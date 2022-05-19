using Interface;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float damage = 150f;
    [SerializeField] private float delay = 2f;
    [SerializeField] private float radius = 5f;
    [SerializeField] private float force = 300f;
    [SerializeField] private ParticleSystem explodeEffect;
    [SerializeField] private AudioClip explodeClip;

    private bool _isExploded;
    private float _countdown;
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        _countdown = delay;
    }

    private void Update()
    {
        _countdown -= Time.deltaTime;

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

    private void Explode()
    {
        if(_isExploded)
            return;

        _isExploded = true;
        
        void ShowEffect()
        {
            if(explodeEffect != null)
                Destroy(
                    Instantiate(explodeEffect.gameObject, transform.position + new Vector3(0, 1.1f, 0), transform.rotation),
                    explodeEffect.main.startLifetime.constantMax
                );
        }

        void AddForce()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

            foreach (var nearbyObjects in colliders)
            {
                if (nearbyObjects.TryGetComponent(out Rigidbody rigidbody))
                {
                    rigidbody.AddExplosionForce(force, transform.position, radius);
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
        
        Destroy(gameObject);
    }
}
