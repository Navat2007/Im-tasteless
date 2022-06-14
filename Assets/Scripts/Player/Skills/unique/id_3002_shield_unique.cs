using System.Collections;
using UnityEngine;

namespace Skills.rare
{
    internal class id_3002_shield_unique : Skill
    {
        [SerializeField] private float invulnerabilityTime = 3;
        [SerializeField] private float reloadTime = 15;
        
        private float _nextInvulnerabilityTime;
        private float _nextLiveArmorReloadTime;

        private void OnDisable()
        {
            if (ControllerManager.playerHealthSystem != null)
            {
                ControllerManager.playerHealthSystem.OnTakeDamage -= OnPlayerDamaged;
            }
        }
        
        private void OnPlayerDamaged(float arg1, float arg2, float arg3)
        {
            if (Time.time > _nextLiveArmorReloadTime)
            {
                StartCoroutine(ArmorActivate());
                _nextInvulnerabilityTime = Time.time + invulnerabilityTime;
                _nextLiveArmorReloadTime = Time.time + reloadTime + invulnerabilityTime;
            }
            
            if (Time.time < _nextInvulnerabilityTime)
            {
                ControllerManager.playerHealthSystem.AddArmor(1);
            }
        }

        private IEnumerator ArmorActivate()
        {
            yield return new WaitForSeconds(reloadTime + invulnerabilityTime);
            ControllerManager.playerHealthSystem.AddArmor(1);
        }

        public override void Activate()
        {
            if (ControllerManager.playerHealthSystem != null)
            {
                ControllerManager.playerHealthSystem.OnTakeDamage += OnPlayerDamaged;
                ControllerManager.playerHealthSystem.AddArmor(1);
            }
        }
    }
}
