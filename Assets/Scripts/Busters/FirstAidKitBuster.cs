using UnityEngine;

namespace Busters
{
    public class FirstAidKitBuster : Buster
    {
        [SerializeField] private float percentToAdd = 25;
        
        public override void PickUp()
        {
            busterController.PickFirstAidKit(count, percentToAdd);
            Destroy(gameObject);
        }
    }
}
