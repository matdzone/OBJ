public class StraightMovement : IMovementStrategy
{
	public void Move(Projectile projectile, double deltaSeconds)
	{
		projectile.MoveBy(
			projectile.VelocityX * deltaSeconds,
			projectile.VelocityY * deltaSeconds
		);
	}
}
