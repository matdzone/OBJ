public class PiercingDecorator : WeaponDecorator
{
    public PiercingDecorator(Weapon weapon)
        : base(weapon)
    {
    }

    public override Projectile Shoot(
        double x,
        double y,
        double directionX,
        double directionY,
        int ownerId,
        GameSession game,
        IMovementStrategy shellMovement)
    {
        Projectile projectile = weapon.Shoot(
            x,
            y,
            directionX,
            directionY,
            ownerId,
            game,
            shellMovement
        );

        projectile.SetPiercing(true);

        return projectile;
    }
}