public class MediumAppearance : TankAppearance
{
    public MediumAppearance() : base("Medium")
    { }

    public override string GetColorHex()   => "#4682B4";
    public override double GetSpriteScale() => 1.05;
    public override string GetTurretStyle() => "medium";
}