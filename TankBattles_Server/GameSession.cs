public class GameSession
{
	public List<Player> Players { get; } = new();
	public List<Projectile> Projectiles { get; } = new();

	public Maze Maze { get; }

	private int _nextPlayerId = 0;

	public GameSession()
	{
		Maze = new Maze();
	}

	public Player AddPlayer(string name)
	{
		Player player = new Player(_nextPlayerId++, name);

		player.Tank.Position.Set(2, 2);

		Players.Add(player);

		return player;
	}

	public Player? GetPlayer(int id)
	{
		return Players.FirstOrDefault(p => p.ID == id);
	}

	public bool MovePlayer(int id, int deltaX, int deltaY)
	{
		Player? player = GetPlayer(id);

		if (player == null)
			return false;

		if (!player.Tank.IsAlive)
			return false;

		int newX = player.X + deltaX;
		int newY = player.Y + deltaY;

		if (!Maze.CanMoveTo(newX, newY))
			return false;

		player.Move(deltaX, deltaY);

		return true;
	}
	public Projectile? Shoot(int playerId)
	{
		Player? player = GetPlayer(playerId);

		if (player == null || !player.Tank.IsAlive)
			return null;

		Projectile projectile =
			player.Tank.Shoot(player.ID);

		Projectiles.Add(projectile);

		return projectile;
	}
	public void UpdateProjectiles()
	{
		for (int i = Projectiles.Count - 1; i >= 0; i--)
		{
			Projectile projectile = Projectiles[i];

			projectile.Move();

			int x = projectile.Position.X;
			int y = projectile.Position.Y;

			if (!Maze.CanMoveTo(x, y))
			{
				Projectiles.RemoveAt(i);
				continue;
			}

			Player? hitPlayer = Players.FirstOrDefault(
				player =>
					player.ID != projectile.OwnerId &&
					player.Tank.IsAlive &&
					player.X == x &&
					player.Y == y
			);

			if (hitPlayer != null)
			{
				hitPlayer.Tank.TakeDamage(projectile.Damage);

				Projectiles.RemoveAt(i);
			}
		}
	}
}