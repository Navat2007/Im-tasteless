public class PickableRifle : PickableWeapon
{
    public override void PickUp()
    {
        weaponController.AddAmmo(count, WeaponType.RIFLE, true);
        Destroy(gameObject);
    }
}
