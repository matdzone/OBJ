public class HealthBox : Box
{
	public const int HealAmount = 50;

	public override string Kind => "Health";

	public HealthBox(int x, int y)
		: base(x, y)
	{
	}

	protected override bool CanApply(Tank tank)
	{
		return tank.Health < Tank.MaxHealth;
	}

	protected override void Apply(Tank tank)
	{
		tank.Heal(HealAmount);
	}
}
