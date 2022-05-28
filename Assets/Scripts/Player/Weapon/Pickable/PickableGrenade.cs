public class PickableGrenade : PickableWeapon
{
    public override void PickUp()
    {
        weaponController.AddAmmo(count, WeaponType.GRENADE, true);
        PickableWeaponPool.Instance.ReturnToPool(this);
    }
}
