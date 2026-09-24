public class Tile
{
	public Position Position { get; }
	public bool IsWalkable { get; protected set; }

	public Tile(int x, int y, bool isWalkable = true)
	{
		Position = new Position(x, y);
		IsWalkable = isWalkable;
	}
}