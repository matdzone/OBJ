public class LightTankFactory : ITankFactory
{
    public Tank CreateTank()
    {
        return new Tank(6.0, 70);
    }
}