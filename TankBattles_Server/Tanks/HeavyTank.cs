public class HeavyTank : Tank
{
    public HeavyTank() : this(new HeavyAppearance()) { }
    public HeavyTank(TankAppearance appearance) : base(3.0, 140, 120.0)
    {
        SetAppearance(appearance);
    }
}