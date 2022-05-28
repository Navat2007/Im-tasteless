public class PickableShotgun : PickableWeapon
{
    public override void PickUp()
    {
        weaponController.AddAmmo(count, WeaponType.SHOTGUN, true);
        PickableWeaponPool.Instance.ReturnToPool(this);
    }
}
