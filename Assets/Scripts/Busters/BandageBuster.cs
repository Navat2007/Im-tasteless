using UnityEngine;

namespace Busters
{
    public class BandageBuster : Buster
    {
        [SerializeField] private float percentToAddOverTime = 1;
        [SerializeField] private float tickTimePeriod = 1;
        [SerializeField] private int tickAmount = 9;
        
        public override void PickUp()
        {
            busterController.PickBandage(count, percentToAddOverTime, tickTimePeriod, tickAmount);
            Destroy(gameObject);
        }
    }
}
