using UnityEngine;

namespace Busters
{
    public class ClipBuster: Buster
    {
        public override void PickUp()
        {
            busterController.PickClip(count);
            Destroy(gameObject);
        }
    }
}