public class LightTank : Tank
{
    public LightTank() : this(new LightAppearance()) { }

    public LightTank(LightAppearance appearance) : base(6.0,70,180.0)
    {
        SetAppearance(appearance);
    }
}