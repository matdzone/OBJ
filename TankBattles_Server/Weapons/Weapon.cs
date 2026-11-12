public class Weapon
{
	public const double ProjectileSpeed = 10.0;

	public virtual string Name { get; }
	public virtual int Damage { get; }

	public virtual bool IsSingleUse => false;

	public Weapon(string name = "Cannon", int damage = 25)
	{
		Name = name;
		Damage = damage;
	}

	public virtual Projectile Shoot(
		double x,
		double y,
		double directionX,
		double directionY,
		int ownerId,
		GameSession game,
		IMovementStrategy shellMovement)
	{
		return CreateProjectile(
			x,
			y,
			directionX,
			directionY,
			ownerId,
			game,
			shellMovement
		);
	}

	protected Projectile CreateProjectile(
		double x,
		double y,
		double directionX,
		double directionY,
		int ownerId,
		GameSession game,
		IMovementStrategy shellMovement)
	{
		IMovementStrategy movement =
			GetMovement(ownerId, x, y, game, shellMovement);

		double speed = GetProjectileSpeed();

		return new Projectile(
			x,
			y,
			directionX * speed,
			directionY * speed,
			Damage,
			ownerId,
			GetProjectileKind(),
			movement,
			GetMaxLifetime()
		);
	}

	protected virtual IMovementStrategy GetMovement(
		int ownerId,
		double x,
		double y,
		GameSession game,
		IMovementStrategy shellMovement)
	{
		return shellMovement;
	}

	protected virtual double GetProjectileSpeed()
	{
		return ProjectileSpeed;
	}

	protected virtual string GetProjectileKind()
	{
		return "Shell";
	}

	protected virtual TimeSpan? GetMaxLifetime()
	{
		return null;
	}
}