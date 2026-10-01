public class SpeedDecorator : WeaponDecorator
{
    public SpeedDecorator(Weapon weapon)
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

        projectile.SetVelocity(
            projectile.VelocityX * 1.5,
            projectile.VelocityY * 1.5
        );

        return projectile;
    }
}