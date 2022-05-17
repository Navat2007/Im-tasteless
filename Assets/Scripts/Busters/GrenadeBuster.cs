namespace Busters
{
    public class GrenadeBuster : Buster
    {
        public override void PickUp()
        {
            busterController.PickGrenade(count);
            Destroy(gameObject);
        }
    }
}