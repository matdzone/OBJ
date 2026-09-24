public class BouncingMovement : IMovementStrategy
{
	public const int MaxBounces = 3;

	private readonly Maze _maze;

	private int _bounces;

	public BouncingMovement(Maze maze)
	{
		_maze = maze;
	}

	public void Move(Projectile projectile, double deltaSeconds)
	{
		double newX = projectile.X + projectile.VelocityX * deltaSeconds;
		double newY = projectile.Y + projectile.VelocityY * deltaSeconds;

		if (IsFree(newX, newY) || _bounces >= MaxBounces)
		{
			projectile.MoveBy(newX - projectile.X, newY - projectile.Y);
			return;
		}

		bool hitVertical = !IsFree(newX, projectile.Y);
		bool hitHorizontal = !IsFree(projectile.X, newY);

		if (!hitVertical && !hitHorizontal)
		{
			hitVertical = true;
			hitHorizontal = true;
		}

		projectile.SetVelocity(
			hitVertical ? -projectile.VelocityX : projectile.VelocityX,
			hitHorizontal ? -projectile.VelocityY : projectile.VelocityY
		);

		_bounces++;
	}

	private bool IsFree(double x, double y)
	{
		return _maze.CanMoveTo((int)Math.Floor(x), (int)Math.Floor(y));
	}
}
