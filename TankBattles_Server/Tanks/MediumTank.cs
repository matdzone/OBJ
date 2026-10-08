public class MediumTank : Tank
{
    public MediumTank() : this(new MediumAppearance()) { }

    public MediumTank(TankAppearance appearance) : base(4.5, 100, 150.0)
    {
        SetAppearance(appearance);
    }
}