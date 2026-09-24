public class GameSession
{
	public List<Player> Players { get; } = new();
	public List<Projectile> Projectiles { get; } = new();

	public Maze Maze { get; }

	private int _nextPlayerId = 0;

	private readonly List<Box> _boxes = new();
	private readonly List<IPlayerMoveObserver> _moveObservers = new();
	private readonly BoxFactory _boxFactory = new();
	private readonly object _boxLock = new();
	private readonly object _updateLock = new();

	private const int MaxBoxes = 2;

	public GameSession()
	{
		Maze = new Maze();
	}

	public List<Box> Boxes
	{
		get
		{
			lock (_boxLock)
				return _boxes.ToList();
		}
	}

	public Player AddPlayer(string name, ITankFactory factory)
	{
		Tank tank = factory.CreateTank();
		Player player = new Player(_nextPlayerId++, name, tank);

		player.Tank.PlaceOnTile(2, 2);

		lock (_updateLock)
			Players.Add(player);

		return player;
	}

	public Player? GetPlayer(int id)
	{
		return Players.FirstOrDefault(p => p.ID == id);
	}

	public Player? FindNearestEnemy(int ownerId, Position from)
	{
		Dictionary<(int X, int Y), int> distances =
			PathFinder.Distances(Maze, from);

		return Players
			.Where(p =>
				p.ID != ownerId &&
				p.Tank.IsAlive &&
				distances.ContainsKey((p.X, p.Y)))
			.OrderBy(p => distances[(p.X, p.Y)])
			.FirstOrDefault();
	}

	public void Subscribe(IPlayerMoveObserver observer)
	{
		lock (_boxLock)
			_moveObservers.Add(observer);
	}

	public void Unsubscribe(IPlayerMoveObserver observer)
	{
		lock (_boxLock)
			_moveObservers.Remove(observer);
	}

	private void NotifyPlayerMoved(Player player)
	{
		List<IPlayerMoveObserver> observers;

		lock (_boxLock)
			observers = _moveObservers.ToList();

		foreach (IPlayerMoveObserver observer in observers)
			observer.OnPlayerMoved(player, this);
	}

	public Box? SpawnBox()
	{
		lock (_boxLock)
		{
			if (_boxes.Count >= MaxBoxes)
				return null;

			Box? box = _boxFactory.Create(Maze, Players, _boxes);

			if (box == null)
				return null;

			_boxes.Add(box);
			Subscribe(box);

			return box;
		}
	}

	public bool RemoveBox(Box box)
	{
		lock (_boxLock)
		{
			Unsubscribe(box);

			return _boxes.Remove(box);
		}
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

		NotifyPlayerMoved(player);

		return true;
	}
	public bool SetInput(int id, int drive, int turn)
	{
		Player? player = GetPlayer(id);

		if (player == null)
			return false;

		player.SetInput(drive, turn);

		return true;
	}

	public bool SetShellMovement(int id, string movement)
	{
		Player? player = GetPlayer(id);

		if (player == null)
			return false;

		return player.Tank.SetShellMovement(movement);
	}

	public void Update(double deltaSeconds)
	{
		lock (_updateLock)
		{
			UpdateTanks(deltaSeconds);
			UpdateProjectiles(deltaSeconds);
		}
	}

	private void UpdateTanks(double deltaSeconds)
	{
		foreach (Player player in Players.ToList())
		{
			if (!player.Tank.IsAlive)
				continue;

			if (player.Tank.Update(deltaSeconds, player.Drive, player.Turn, Maze))
				NotifyPlayerMoved(player);
		}
	}

	public Projectile? Shoot(int playerId)
	{
		Player? player = GetPlayer(playerId);

		if (player == null || !player.Tank.IsAlive)
			return null;

		lock (_updateLock)
		{
			Projectile projectile =
				player.Tank.Shoot(player.ID, this);

			Projectiles.Add(projectile);

			return projectile;
		}
	}
	private void UpdateProjectiles(double deltaSeconds)
	{
		for (int i = Projectiles.Count - 1; i >= 0; i--)
		{
			Projectile projectile = Projectiles[i];

			if (projectile.IsExpired || TryHit(projectile))
			{
				Projectiles.RemoveAt(i);
				continue;
			}

			projectile.Move(deltaSeconds);

			if (!Maze.CanMoveTo(projectile.TileX, projectile.TileY))
			{
				Projectiles.RemoveAt(i);
				continue;
			}

			if (TryHit(projectile))
				Projectiles.RemoveAt(i);
		}
	}

	private bool TryHit(Projectile projectile)
	{
		double hitDistance = Tank.Radius + Projectile.Radius;

		Player? hitPlayer = Players.FirstOrDefault(
			player =>
				player.ID != projectile.OwnerId &&
				player.Tank.IsAlive &&
				Math.Pow(player.Tank.X - projectile.X, 2) +
				Math.Pow(player.Tank.Y - projectile.Y, 2) <
				hitDistance * hitDistance
		);

		if (hitPlayer == null)
			return false;

		hitPlayer.Tank.TakeDamage(projectile.Damage);

		return true;
	}
}
