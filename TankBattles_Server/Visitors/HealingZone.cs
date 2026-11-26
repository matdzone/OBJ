public class HealingZone
{
	private const double Interval = 1.0;
	private readonly HealingZoneVisitor _visitor;
	private double _timer;

	public HealingZone(Maze maze)
	{
		_visitor = new HealingZoneVisitor(maze);
	}

	public void Update(double deltaSeconds, IEnumerable<Player> players)
	{
		_timer += deltaSeconds;
		if (_timer < Interval) return;

		_timer -= Interval;

		foreach (Player player in players.ToList())
			player.Tank.Accept(_visitor);
	}
}