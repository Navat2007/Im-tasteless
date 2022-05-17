using UnityEngine;

namespace Busters
{
    public class AttackSpeedBuster : Buster
    {
        [SerializeField] private float speedPercent = 50;
        [SerializeField] private float duration = 10;
        
        public override void PickUp()
        {
            busterController.PickAttackSpeed(count, duration, speedPercent);
            Destroy(gameObject);
        }
    }
}