public class Wall : Tile
{
    public WallType Type { get; }

    public Wall(int x, int y, WallType type)
        : base(x, y, type.IsWalkable)
    {
        Type = type;
    }
}