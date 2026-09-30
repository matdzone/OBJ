public class Maze
{
	public int Width { get; }
	public int Height { get; }

    public IReadOnlyList<Wall> Walls { get; }
	public Maze(List<Wall> walls, int width = 40, int height = 20)
	{
		Width = width;
		Height = height;

		Walls = walls.ToList();

	
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