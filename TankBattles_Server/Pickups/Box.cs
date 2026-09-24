public abstract class Box : IPlayerMoveObserver
{
	public Position Position { get; }

	public abstract string Kind { get; }

	protected Box(int x, int y)
	{
		Position = new Position(x, y);
	}

	public void OnPlayerMoved(Player player, GameSession game)
	{
		if (player.X != Position.X || player.Y != Position.Y)
			return;

		if (!CanApply(player.Tank))
			return;

		if (!game.RemoveBox(this))
			return;

		Apply(player.Tank);
	}

	protected abstract bool CanApply(Tank tank);

	protected abstract void Apply(Tank tank);
}
