public class Player
{
	private readonly List<IPlayerMoveObserver> moveObservers =
		new List<IPlayerMoveObserver>();

	public int ID { get; }
	public string Name { get; }
	public Tank Tank { get; }

	public bool IsReady { get; private set; }

	public int X => Tank.TileX;
	public int Y => Tank.TileY;

	public int Drive { get; private set; }
	public int Turn { get; private set; }

	public Player(int id, string name, Tank tank)
	{
		ID = id;
		Name = name;
		Tank = tank;
	}

	public void Move(int deltaX, int deltaY)
	{
		Tank.Move(deltaX, deltaY);

		NotifyMoveObservers();
	}

	public void AddMoveObserver(IPlayerMoveObserver observer)
	{
		if (!moveObservers.Contains(observer))
		{
			moveObservers.Add(observer);
		}
	}

	public void RemoveMoveObserver(IPlayerMoveObserver observer)
	{
		moveObservers.Remove(observer);
	}

	private void NotifyMoveObservers()
	{
		List<IPlayerMoveObserver> currentObservers =
			new List<IPlayerMoveObserver>(moveObservers);

		foreach (IPlayerMoveObserver observer in currentObservers)
		{
			observer.OnPlayerMoved(this);
		}
	}
	public bool Update(double deltaSeconds, Maze maze)
	{
		bool moved = Tank.Update(deltaSeconds, Drive, Turn, maze);

		if (moved)
			NotifyMoveObservers();

		return moved;
	}

	public void SetInput(int drive, int turn)
	{
		Drive = Math.Clamp(drive, -1, 1);
		Turn = Math.Clamp(turn, -1, 1);
	}

	public void SetReady(bool ready)
	{
		IsReady = ready;
	}
}