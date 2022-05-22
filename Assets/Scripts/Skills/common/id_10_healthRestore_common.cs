using System.Collections;
using UnityEngine;

namespace Skills.common
{
    internal class id_10_healthRestore_common : Skill
    {
        [SerializeField] private float invulnerabilityTime = 5;
        [SerializeField] private float radius = 15;
        [SerializeField] private float force = 100;
        [SerializeField] private float upwardsModifier = 0;
        [SerializeField] private ForceMode forceMode;
        
        public override void Activate()
        {
            void AddForce()
            {
                Collider[] hitColliders = Physics.OverlapSphere(ControllerManager.player.transform.position, radius);

                foreach (var item in hitColliders)
                {
                    if (item.TryGetComponent(out EnemyController enemy) && item.TryGetComponent(out Rigidbody rigidbody))
                    {
                        enemy.StopNavMeshAgent();
                        rigidbody.AddExplosionForce(ControllerManager.skillController.IsNextDouble 
                                ? force * 2 : force, 
                            ControllerManager.player.transform.position, 
                            ControllerManager.skillController.IsNextDouble 
                                ? radius * 2 : radius, upwardsModifier, forceMode);
                        
                        enemy.StartNavMeshAgent(2);
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
