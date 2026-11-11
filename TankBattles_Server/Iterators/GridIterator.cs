public class GridIterator : IIterator<Position>
{
	private readonly bool[,] _cells;
	private int _x;
	private int _y;

	public GridIterator(bool[,] cells)
	{
		_cells = cells;
	}

	public bool HasNext()
	{
		SkipBlocked();

		return _y < _cells.GetLength(1);
	}

	public Position Next()
	{
		if (!HasNext())
			throw new InvalidOperationException("No more walkable tiles.");

		Position position = new Position(_x, _y);
		Advance();

		return position;
	}

	public void Reset()
	{
		_x = 0;
		_y = 0;
	}

	private void SkipBlocked()
	{
		while (_y < _cells.GetLength(1) && !_cells[_x, _y])
			Advance();
	}

	private void Advance()
	{
		_x++;

		if (_x >= _cells.GetLength(0))
		{
			_x = 0;
			_y++;
		}
	}
}
