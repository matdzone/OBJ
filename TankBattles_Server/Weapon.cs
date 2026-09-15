public class Weapon
{
	public string Name { get; }
	public int Damage { get; }

	public Weapon(string name = "Cannon", int damage = 25)
	{
		Name = name;
		Damage = damage;
	}

	public Projectile Shoot(Position position, int deltaX, int deltaY, int ownerId)
	{
		return new Projectile(
			position.X,
			position.Y,
			deltaX,
			deltaY,
			Damage,
			ownerId
		);
	}
}