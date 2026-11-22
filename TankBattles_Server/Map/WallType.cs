public class WallType
{
    public string Name { get; }
    public bool IsWalkable { get; }

    public byte[] TextureData { get; }
    
    public WallType(string name, bool isWalkable, byte[] textureData)
    {
        Name = name;
        IsWalkable = isWalkable;
        TextureData = textureData;
    }
}