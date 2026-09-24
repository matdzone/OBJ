public class HeavyTankFactory : ITankFactory
{
    public Tank CreateTank()
    {
        return new Tank(3.0, 140);
    }
}