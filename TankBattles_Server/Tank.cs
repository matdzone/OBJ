public class Tank
{
	public Position Position { get; }

	public int Health { get; private set; }

	public Weapon Weapon { get; }

	public int DirectionX { get; private set; } = 1;
	public int DirectionY { get; private set; } = 0;

	public bool IsAlive => Health > 0;

	public Tank()
	{
		Position = new Position(0, 0);
		Health = 100;
		Weapon = new Weapon();
	}

	public void Move(int deltaX, int deltaY)
	{
		if (deltaX != 0 || deltaY != 0)
		{
			DirectionX = deltaX;
			DirectionY = deltaY;
		}

		Position.Move(deltaX, deltaY);
	}

	public Projectile Shoot(int ownerId)
	{
		return Weapon.Shoot(
			Position,
			DirectionX,
			DirectionY,
			ownerId
		);
	}

	public void TakeDamage(int damage)
	{
		Health = Math.Max(0, Health - damage);
	}
}