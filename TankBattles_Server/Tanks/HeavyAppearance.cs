public class HeavyAppearance : TankAppearance
{
   public HeavyAppearance() : base("Heavy")
   {}
   public override string GetColorHex()   => "#556B2F";
    public override double GetSpriteScale() => 1.25;
    public override string GetTurretStyle() => "wide";
}