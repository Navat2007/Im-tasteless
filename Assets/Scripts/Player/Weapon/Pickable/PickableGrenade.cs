public class PickableGrenade : PickableWeapon
{
    public override void PickUp()
    {
        weaponController.AddAmmo(count, WeaponType.GRENADE, true);
        
        if(ControllerManager.player.HealOnTakeAmmoPercent != 0)
            ControllerManager.playerHealthSystem.AddHealthPercent(ControllerManager.player.HealOnTakeAmmoPercent);
        
        PickableWeaponPool.Instance.ReturnToPool(this);
    }
}
