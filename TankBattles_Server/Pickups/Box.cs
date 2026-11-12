public abstract class Box : IPlayerMoveObserver
{
	public Position Position { get; }
	public abstract string Kind { get; }

	protected Box(int x, int y)
	{
		Position = new Position(x, y);
	}

	public void OnPlayerMoved(Player player)
	{
		if (player.X == Position.X && player.Y == Position.Y)
			Collect(player.Tank);
	}

	public void Collect(Tank tank)
	{
		if (!CanApply(tank))
			return;

		GameSession game = GameManager.Instance.GameSession;

		if (!game.RemoveBox(this))
			return;

		Apply(tank);
	}

	protected abstract bool CanApply(Tank tank);
	protected abstract void Apply(Tank tank);
}