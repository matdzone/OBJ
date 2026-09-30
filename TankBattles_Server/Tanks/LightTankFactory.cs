public class LightTankFactory : ITankFactory
{
    public Tank CreateTank()
    {
        return new LightTank();
    }
    public Weapon CreateWeapon()
    {
        return new LightWeapon();
    }
    public TankAppearance CreateAppearance()
    {
        return new LightAppearance();
    }
}