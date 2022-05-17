namespace Busters
{
    public class AttackSpeedBuster : Buster
    {
        public override void PickUp()
        {
            busterController.PickAttackSpeed(count);
            Destroy(gameObject);
        }
    }
}