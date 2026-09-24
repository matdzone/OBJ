public class RocketBox : Box
{
	public override string Kind => "Rocket";

	public RocketBox(int x, int y)
		: base(x, y)
	{
	}

	protected override bool CanApply(Tank tank)
	{
		return !tank.Weapon.IsSingleUse;
	}

	protected override void Apply(Tank tank)
	{
		tank.EquipWeapon(new RocketWeapon());
	}
}
