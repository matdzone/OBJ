public class HomingMovement : IMovementStrategy
{
	private readonly Maze _maze;
	private readonly Player _target;
	private readonly StraightMovement _fallback = new();

	public HomingMovement(Maze maze, Player target)
	{
		_maze = maze;
		_target = target;
	}

	public void Move(Projectile projectile, double deltaSeconds)
	{
		if (!_target.Tank.IsAlive)
		{
			_fallback.Move(projectile, deltaSeconds);
			return;
		}

		double waypointX = _target.Tank.X;
		double waypointY = _target.Tank.Y;

		(int DeltaX, int DeltaY)? step =
			PathFinder.NextStep(
				_maze,
				new Position(projectile.TileX, projectile.TileY),
				_target.Tank.Tile
			);

		if (step != null)
		{
			waypointX = projectile.TileX + step.Value.DeltaX + 0.5;
			waypointY = projectile.TileY + step.Value.DeltaY + 0.5;
		}

		double dx = waypointX - projectile.X;
		double dy = waypointY - projectile.Y;
		double length = Math.Sqrt(dx * dx + dy * dy);

		if (length > 0.0001)
		{
			double speed = projectile.Speed;

			projectile.SetVelocity(dx / length * speed, dy / length * speed);
		}

		_fallback.Move(projectile, deltaSeconds);
	}
}
