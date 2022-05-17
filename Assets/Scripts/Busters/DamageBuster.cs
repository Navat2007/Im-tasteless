using UnityEngine;

namespace Busters
{
    public class DamageBuster : Buster
    {
        [SerializeField] private float damagePercent = 50;
        [SerializeField] private float duration = 10;
        
        public override void PickUp()
        {
            busterController.PickDamage(count, duration, damagePercent);
            Destroy(gameObject);
        }
    }
}