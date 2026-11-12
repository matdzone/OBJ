public class WallTypeFactory
{
    private readonly Dictionary<string, WallType> _types = new();

    public WallType GetWallType(string name, bool isWalkable)
    {
        if (!_types.TryGetValue(name, out WallType? wallType))
        {
            wallType = new WallType(name, isWalkable);
            _types.Add(name, wallType);
        }

        return wallType;
    }
}