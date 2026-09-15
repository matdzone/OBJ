public class Projectile
{
	public Position Position { get; }
	public int DeltaX { get; }
	public int DeltaY { get; }
	public int Damage { get; }
	public int OwnerId { get; }

	public Projectile(
		int x,
		int y,
		int deltaX,
		int deltaY,
		int damage,
		int ownerId)
	{
		Position = new Position(x, y);
		DeltaX = deltaX;
		DeltaY = deltaY;
		Damage = damage;
		OwnerId = ownerId;
	}

	public void Move()
	{
		Position.Move(DeltaX, DeltaY);
	}
}