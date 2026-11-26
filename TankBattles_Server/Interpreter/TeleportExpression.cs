
public class TeleportExpression : IExpression
{
    private readonly int _playerId;
    private readonly int _x;
    private readonly int _y;

    public TeleportExpression(int playerId, int x, int y)
    {
        _playerId = playerId;
        _x = x;
        _y = y;
    }

    public void Interpret(InterpreterContext context)
    {
        Player? player = context.Game.GetPlayer(_playerId);

        if (player == null)
        {
            Console.WriteLine("Player not found.");
            return;
        }

        if (!player.Tank.IsAlive)
        {
            Console.WriteLine("Player is dead.");
            return;
        }

        if (!context.Game.Maze.CanMoveTo(_x, _y))
        {
            Console.WriteLine("Invalid teleport position.");
            return;
        }

        player.Tank.PlaceOnTile(_x, _y);

        Console.WriteLine(
            $"Player {_playerId} teleported to ({_x}, {_y})."
        );
    }
}
