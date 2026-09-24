public class Position
{
	public int X { get; private set; }
	public int Y { get; private set; }

	public Position(int x = 0, int y = 0)
	{
		X = x;
		Y = y;
	}

	public void Move(int deltaX, int deltaY)
	{
		X += deltaX;
		Y += deltaY;
	}

	public void Set(int x, int y)
	{
		X = x;
		Y = y;
	}
}