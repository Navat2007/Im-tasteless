namespace Busters
{
    public class BodyArmorBuster : Buster
    {
        public override void PickUp()
        {
            busterController.PickBodyArmor(count);
            Destroy(gameObject);
        }
    }
}