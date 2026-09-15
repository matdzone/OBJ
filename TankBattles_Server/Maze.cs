public class Maze
{
	public int Width { get; }
	public int Height { get; }

	public List<Wall> Walls { get; } = new();

	public Maze(int width = 40, int height = 20)
	{
		Width = width;
		Height = height;

		CreateBorderWalls();
	}

	private void CreateBorderWalls()
	{
		for (int x = 0; x < Width; x++)
		{
			Walls.Add(new Wall(x, 0));
			Walls.Add(new Wall(x, Height - 1));
		}

		for (int y = 1; y < Height - 1; y++)
		{
			Walls.Add(new Wall(0, y));
			Walls.Add(new Wall(Width - 1, y));
		}
	}

	public bool CanMoveTo(int x, int y)
	{
		if (x < 0 || x >= Width || y < 0 || y >= Height)
			return false;

		return !Walls.Any(w =>
			w.Position.X == x &&
			w.Position.Y == y);
	}
}