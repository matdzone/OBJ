public abstract class TankAppearance
{
    public string Type {get; private set;}
    public TankAppearance(string type)
    {
        Type = type;
    }

    public abstract string GetColorHex();
    public abstract double GetSpriteScale();
    public abstract string GetTurretStyle();
}