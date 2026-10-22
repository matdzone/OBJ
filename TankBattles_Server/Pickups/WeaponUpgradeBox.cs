public class WeaponUpgradeBox : Box
{
    public override string Kind => "WeaponUpgrade";

    public WeaponUpgradeBox(int x, int y)
        : base(x, y)
    {
    }

    protected override bool CanApply(Tank tank)
    {
        return tank.WeaponUpgradeLevel < 3;
    }

    protected override void Apply(Tank tank)
    {
        tank.UpgradeWeapon();
    }
}