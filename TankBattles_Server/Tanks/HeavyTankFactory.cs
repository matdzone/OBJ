public class HeavyTankFactory : ITankFactory
{
    public Tank CreateTank()
    {
        return new HeavyTank();
    }
    public Weapon CreateWeapon()
    {
        return new HeavyWeapon();
    }
    public TankAppearance CreateAppearance()
    {
        return new HeavyAppearance();
    }
}