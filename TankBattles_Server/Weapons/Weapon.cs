public class Weapon
{
	public const double ProjectileSpeed = 10.0;

	public string Name { get; }
	public int Damage { get; }

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
		GameSession game)
	{
		return new Projectile(
			x,
			y,
			directionX * ProjectileSpeed,
			directionY * ProjectileSpeed,
			Damage,
			ownerId
		);
	}
}
