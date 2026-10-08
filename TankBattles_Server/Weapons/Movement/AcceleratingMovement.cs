public class AcceleratingMovement : IMovementStrategy
{
	public const double StartSpeed = 2.0;
	public const double Acceleration = 12.0;
	public const double MaxSpeed = 18.0;

	private readonly StraightMovement _straight = new();

	private bool _started;

	public void Move(Projectile projectile, double deltaSeconds)
	{
		double speed = projectile.Speed;

		if (speed < 0.0001)
			return;

		double newSpeed = _started
			? Math.Min(MaxSpeed, speed + Acceleration * deltaSeconds)
			: StartSpeed;

		_started = true;

		projectile.SetVelocity(
			projectile.VelocityX / speed * newSpeed,
			projectile.VelocityY / speed * newSpeed
		);

		_straight.Move(projectile, deltaSeconds);
	}
}
