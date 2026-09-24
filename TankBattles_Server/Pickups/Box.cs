public class Box : IPlayerMoveObserver
{
	public Position Position { get; }

	public Box(int x, int y)
	{
		Position = new Position(x, y);
	}

	public void OnPlayerMoved(Player player, GameSession game)
	{
		if (player.X != Position.X || player.Y != Position.Y)
			return;

		if (player.Tank.Weapon.IsSingleUse)
			return;

		if (!game.RemoveBox(this))
			return;

		player.Tank.EquipWeapon(new RocketWeapon());
	}
}
