public sealed class RocketWeapon : Weapon
{
	private static readonly TimeSpan MaxFlightTime =
		TimeSpan.FromSeconds(5);

	public const double RocketSpeed = 5.0;

	public override bool IsSingleUse => true;

	public RocketWeapon()
		: base("Rocket", 50)
	{
	}

	protected override IMovementStrategy GetMovement(
		int ownerId,
		double x,
		double y,
		GameSession game,
		IMovementStrategy shellMovement)
	{
		Player? target = game.FindNearestEnemy(
			ownerId,
			new Position(
				(int)Math.Floor(x),
				(int)Math.Floor(y))
		);

		return target == null
			? new StraightMovement()
			: new HomingMovement(game.Maze, target);
	}

	protected override double GetProjectileSpeed()
	{
		return RocketSpeed;
	}

	protected override string GetProjectileKind()
	{
		return "Rocket";
	}

	protected override TimeSpan? GetMaxLifetime()
	{
		return MaxFlightTime;
	}
}