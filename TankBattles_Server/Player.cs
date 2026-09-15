public class Player
{
	public int ID { get; }
	public string Name { get; }
	public Tank Tank { get; }

	public bool IsReady { get; private set; }

	public int X => Tank.Position.X;
	public int Y => Tank.Position.Y;

	public Player(int id, string name)
	{
		ID = id;
		Name = name;
		Tank = new Tank();
	}

	public void Move(int deltaX, int deltaY)
	{
		Tank.Move(deltaX, deltaY);
	}

	public void SetReady(bool ready)
	{
		IsReady = ready;
	}
}