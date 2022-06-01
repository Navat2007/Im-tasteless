public class PickableRifle : PickableWeapon
{
    public override void PickUp()
    {
        weaponController.AddAmmo(count, WeaponType.RIFLE, true);
        
        if(ControllerManager.player.HealOnTakeAmmoPercent != 0)
            ControllerManager.healthSystem.AddHealthPercent(ControllerManager.player.HealOnTakeAmmoPercent);
        
        PickableWeaponPool.Instance.ReturnToPool(this);
    }
}
