public class BoxFactory
{
	private readonly Random _random = new();

	public Box? Create(
		Maze maze,
		IEnumerable<Player> players,
		IEnumerable<Box> existingBoxes)
	{
		HashSet<(int X, int Y)> occupied = new();

		foreach (Player player in players)
			occupied.Add((player.X, player.Y));

		foreach (Box box in existingBoxes)
			occupied.Add((box.Position.X, box.Position.Y));

		List<(int X, int Y)> freeTiles = new();

		IIterator<Position> tiles = new MazeGrid(maze).CreateIterator();

		while (tiles.HasNext())
		{
			Position tile = tiles.Next();

			if (!occupied.Contains((tile.X, tile.Y)))
				freeTiles.Add((tile.X, tile.Y));
		}

		if (freeTiles.Count == 0)
			return null;

		(int X, int Y) spot = freeTiles[_random.Next(freeTiles.Count)];

		return _random.Next(5) switch
		{
			0 => new RocketBox(spot.X, spot.Y),
			1 => new HealthBox(spot.X, spot.Y),
			2 => new DisguiseBox(spot.X, spot.Y),
			3 => new ShieldBox(spot.X, spot.Y),
			_ => new WeaponUpgradeBox(spot.X, spot.Y)
		};
	}
}
