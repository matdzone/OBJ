public sealed class RespawningState : TankState
{
	private double _timeLeft = 2;

	public override bool CanShoot => false;
	public override void TakeDamage(Tank tank, int damage) { }

	public override void Update(Tank tank, double deltaSeconds)
	{
		_timeLeft -= deltaSeconds;
		if (_timeLeft <= 0) tank.TransitionTo(new ActiveState());
	}
}