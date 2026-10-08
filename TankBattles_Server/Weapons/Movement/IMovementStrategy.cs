public interface IMovementStrategy
{
	void Move(Projectile projectile, double deltaSeconds);

	IMovementStrategy Clone();

}
