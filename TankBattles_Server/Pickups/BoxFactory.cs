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

		for (int x = 0; x < maze.Width; x++)
		{
			for (int y = 0; y < maze.Height; y++)
			{
				if (maze.CanMoveTo(x, y) && !occupied.Contains((x, y)))
					freeTiles.Add((x, y));
			}
		}

		if (freeTiles.Count == 0)
			return null;

		(int X, int Y) tile = freeTiles[_random.Next(freeTiles.Count)];

		return _random.Next(3) switch
		{
			0 => new RocketBox(tile.X, tile.Y),
			1 => new HealthBox(tile.X, tile.Y),
			_ => new ShieldBox(tile.X, tile.Y)
		};
	}
}
