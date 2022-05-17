namespace Busters
{
    public class MoveSpeedBuster : Buster
    {
        public override void PickUp()
        {
            busterController.PickMoveSpeed(count);
            Destroy(gameObject);
        }
    }
}