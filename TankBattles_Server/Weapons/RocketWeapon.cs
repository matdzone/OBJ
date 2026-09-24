public class RocketWeapon : Weapon
{
	private static readonly TimeSpan MaxFlightTime =
		TimeSpan.FromSeconds(5);

	public const double RocketSpeed = 5.0;

	public override bool IsSingleUse => true;

	public RocketWeapon()
		: base("Rocket", 50)
	{
	}

	public override Projectile Shoot(
		double x,
		double y,
		double directionX,
		double directionY,
		int ownerId,
		GameSession game,
		IMovementStrategy shellMovement)
	{
		Player? target = game.FindNearestEnemy(
			ownerId,
			new Position((int)Math.Floor(x), (int)Math.Floor(y))
		);

		IMovementStrategy movement =
			target == null
				? new StraightMovement()
				: new HomingMovement(game.Maze, target);

		return new Projectile(
			x,
			y,
			directionX * RocketSpeed,
			directionY * RocketSpeed,
			Damage,
			ownerId,
			"Rocket",
			movement,
			MaxFlightTime
		);
	}
}
