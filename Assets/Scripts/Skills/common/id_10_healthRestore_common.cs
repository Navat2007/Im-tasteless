using UnityEngine;

namespace Skills.common
{
    internal class id_10_healthRestore_common : Skill
    {
        [SerializeField] private float invulnerabilityTime = 5;
        [SerializeField] private float radius = 5;
        [SerializeField] private float force = 700;
        
        public override void Activate()
        {
            void AddForce()
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

                foreach (var nearbyObjects in colliders)
                {
                    if (nearbyObjects.TryGetComponent(out Rigidbody rigidbody))
                    {
                        rigidbody.AddExplosionForce(ControllerManager.skillController.IsNextDouble 
                            ? force * 2 : force, 
                            transform.position, 
                            ControllerManager.skillController.IsNextDouble 
                            ? radius * 2 : radius);
                    }
                }
            }
            
            ControllerManager.healthSystem.AddHealth(ControllerManager.healthSystem.MaxHealth);
            ControllerManager.healthSystem.AddNextInvulnerabilityTime(ControllerManager.skillController.IsNextDouble 
                ? invulnerabilityTime * 2 : invulnerabilityTime);
            AddForce();
        }
    }
}
