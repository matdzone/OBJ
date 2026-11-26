public sealed class CriticalState : TankState
{
	public override double SpeedMultiplier => 0.5;

	public override void Heal(Tank tank, int amount)
	{
		tank.ApplyHeal(amount);
		if (!IsCritical(tank)) tank.TransitionTo(new ActiveState());
	}
}