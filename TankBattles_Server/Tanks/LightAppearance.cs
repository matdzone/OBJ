public class LightAppearance : TankAppearance
{
    public LightAppearance() : base("Light")
    {
    }
    public override string GetColorHex()   => "#DAA520";
    public override double GetSpriteScale() => 0.90;
    public override string GetTurretStyle() => "slim";
}