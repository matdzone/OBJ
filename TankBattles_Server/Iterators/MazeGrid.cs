public class MazeGrid : IIterableCollection<Position>
{
	private readonly bool[,] _walkable;

	public int Width { get; }
	public int Height { get; }

	public MazeGrid(Maze maze)
	{
		Width = maze.Width;
		Height = maze.Height;
		_walkable = new bool[Width, Height];

		for (int x = 0; x < Width; x++)
			for (int y = 0; y < Height; y++)
				_walkable[x, y] = maze.CanMoveTo(x, y);
	}

	public IIterator<Position> CreateIterator()
	{
		return new GridIterator(_walkable);
	}
}
