public class StraightMovement : IMovementStrategy
{
	public IMovementStrategy Clone() => this; 
	public void Move(Projectile projectile, double deltaSeconds)
	{
		projectile.MoveBy(
			projectile.VelocityX * deltaSeconds,
			projectile.VelocityY * deltaSeconds
		);
	}
}
