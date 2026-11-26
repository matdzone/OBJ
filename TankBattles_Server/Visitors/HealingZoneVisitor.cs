public class HealingZoneVisitor : IGameVisitor
{
	public const double Radius = 2.5;
	private const int HealAmount = 5;

	private readonly double _centerX;
	private readonly double _centerY;

	public HealingZoneVisitor(Maze maze)
	{
		_centerX = maze.Width / 2.0;
		_centerY = maze.Height / 2.0;
	}

	public void Visit(Tank tank)
	{
		double dx = tank.X - _centerX;
		double dy = tank.Y - _centerY;

		if (dx * dx + dy * dy <= Radius * Radius)
			tank.Heal(HealAmount);
	}

	public void Visit(Wall wall) { }
	public void Visit(Projectile projectile) { }
}