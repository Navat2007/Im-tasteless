namespace Busters
{
    public class BodyArmorBuster : Buster
    {
        public override void PickUp()
        {
            busterController.PickBodyArmor(count);
            BusterPool.Instance.ReturnToPool(this);
        }
    }
}