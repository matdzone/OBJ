public class Tank
{
	public const double Radius = 0.35;
	public const double Speed = 4.0;
	public const double TurnSpeed = 180.0;

	public double X { get; private set; }
	public double Y { get; private set; }

	public double Angle { get; private set; }

	public int Health { get; private set; }

	public Weapon Weapon { get; private set; }

	public double DirectionX => Math.Cos(Angle * Math.PI / 180);
	public double DirectionY => Math.Sin(Angle * Math.PI / 180);

	public int TileX => (int)Math.Floor(X);
	public int TileY => (int)Math.Floor(Y);

	public Position Tile => new Position(TileX, TileY);

	public bool IsAlive => Health > 0;

	public Tank()
	{
		Health = 100;
		Weapon = new Weapon();
	}

	public void PlaceOnTile(int tileX, int tileY)
	{
		X = tileX + 0.5;
		Y = tileY + 0.5;
	}

	public void Move(int deltaX, int deltaY)
	{
		if (deltaX != 0 || deltaY != 0)
			Angle = Math.Atan2(deltaY, deltaX) * 180 / Math.PI;

		X += deltaX;
		Y += deltaY;
	}

	public bool Update(double deltaSeconds, int drive, int turn, Maze maze)
	{
		Angle = ((Angle + turn * TurnSpeed * deltaSeconds) % 360 + 360) % 360;

		if (drive == 0)
			return false;

		double distance = drive * Speed * deltaSeconds;

		double newX = X + DirectionX * distance;
		double newY = Y + DirectionY * distance;

		bool moved = false;

		if (!CollidesWithWall(maze, newX, Y))
		{
			X = newX;
			moved = true;
		}

		if (!CollidesWithWall(maze, X, newY))
		{
			Y = newY;
			moved = true;
		}

		return moved;
	}

	private static bool CollidesWithWall(Maze maze, double x, double y)
	{
		for (int tileX = (int)Math.Floor(x - Radius); tileX <= (int)Math.Floor(x + Radius); tileX++)
		{
			for (int tileY = (int)Math.Floor(y - Radius); tileY <= (int)Math.Floor(y + Radius); tileY++)
			{
				if (maze.CanMoveTo(tileX, tileY))
					continue;

				double closestX = Math.Clamp(x, tileX, tileX + 1);
				double closestY = Math.Clamp(y, tileY, tileY + 1);

				double dx = x - closestX;
				double dy = y - closestY;

				if (dx * dx + dy * dy < Radius * Radius)
					return true;
			}
		}

		return false;
	}

	public void EquipWeapon(Weapon weapon)
	{
		Weapon = weapon;
	}

	public Projectile Shoot(int ownerId, GameSession game)
	{
		Projectile projectile = Weapon.Shoot(
			X,
			Y,
			DirectionX,
			DirectionY,
			ownerId,
			game
		);

		if (Weapon.IsSingleUse)
			Weapon = new Weapon();

		return projectile;
	}

	public void TakeDamage(int damage)
	{
		Health = Math.Max(0, Health - damage);
	}
}
