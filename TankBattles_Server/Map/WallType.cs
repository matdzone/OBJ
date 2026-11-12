public class WallType
{
    public string Name { get; }
    public bool IsWalkable { get; }

    public WallType(string name, bool isWalkable)
    {
        Name = name;
        IsWalkable = isWalkable;
    }
}