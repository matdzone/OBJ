public class Player
{
	public int ID { get; }
	public string Name { get; }
	public Tank Tank { get; }

	public bool IsReady { get; private set; }

	public int X => Tank.TileX;
	public int Y => Tank.TileY;

	public int Drive { get; private set; }
	public int Turn { get; private set; }

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

	public void SetInput(int drive, int turn)
	{
		Drive = Math.Clamp(drive, -1, 1);
		Turn = Math.Clamp(turn, -1, 1);
	}

	public void SetReady(bool ready)
	{
		IsReady = ready;
	}
}