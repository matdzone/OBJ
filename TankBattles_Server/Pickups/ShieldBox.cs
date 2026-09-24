public class ShieldBox : Box
{
	public const int ShieldAmount = 50;

	public override string Kind => "Shield";

	public ShieldBox(int x, int y)
		: base(x, y)
	{
	}

	protected override bool CanApply(Tank tank)
	{
		return tank.Shield == 0;
	}

	protected override void Apply(Tank tank)
	{
		tank.AddShield(ShieldAmount);
	}
}
