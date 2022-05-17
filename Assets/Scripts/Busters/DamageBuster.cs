namespace Busters
{
    public class DamageBuster : Buster
    {
        public override void PickUp()
        {
            busterController.PickDamage(count);
            Destroy(gameObject);
        }
    }
}