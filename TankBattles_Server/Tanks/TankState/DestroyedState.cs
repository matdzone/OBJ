public sealed class DestroyedState : TankState
{
	private double _timeLeft = 3;

	public override double SpeedMultiplier => 0;
	public override bool CanShoot => false;
	public override void TakeDamage(Tank tank, int damage) { }
	public override void Heal(Tank tank, int amount) { }

	public override void Update(Tank tank, double deltaSeconds)
	{
		_timeLeft -= deltaSeconds;
		if (_timeLeft > 0) return;

		tank.RestoreHealth();
		tank.TransitionTo(new RespawningState());
	}
}