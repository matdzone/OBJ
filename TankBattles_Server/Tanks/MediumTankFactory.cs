public class MediumTankFactory : ITankFactory
{
    public Tank CreateTank() => new MediumTank();
    public Weapon CreateWeapon() => new MediumWeapon();
    public TankAppearance CreateAppearance() => new MediumAppearance();
}