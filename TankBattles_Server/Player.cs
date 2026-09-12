public class Player
{
    public int ID { get; }
    public string Name { get; }

    public int X { get; private set; }
    public int Y { get; private set; }

    public Player(int id, string name)
    {
        ID = id;
        Name = name;
        X = 0;
        Y = 0;
    }
    public void Move(int deltaX, int deltaY)
    {
        X += deltaX;
        Y += deltaY;
    }
}