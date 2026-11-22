
public class WallTypeFactory
{
    private readonly Dictionary<string, WallType> _types = new();

    public WallType GetWallType(string name, bool isWalkable)
    {
        if (!_types.TryGetValue(name, out WallType? wallType))
        {
            // Sukuriame 1 MiB tekstūros duomenų
            byte[] textureData = new byte[1024 * 1024];

            wallType = new WallType(name, isWalkable, textureData);
            _types.Add(name, wallType);
        }

        return wallType;
    }
}
