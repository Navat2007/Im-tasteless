using UnityEngine;

namespace Busters
{
    public class MoveSpeedBuster : Buster
    {
        [SerializeField] private float moveSpeedPercent = 50;
        [SerializeField] private float duration = 10;
        
        public override void PickUp()
        {
            busterController.PickMoveSpeed(count, duration, moveSpeedPercent);
            Destroy(gameObject);
        }
    }
}