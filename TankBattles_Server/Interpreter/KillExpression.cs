
public class KillExpression : IExpression
{
    private readonly int _playerId;

    public KillExpression(int playerId)
    {
        _playerId = playerId;
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
            Console.WriteLine("Player is already dead.");
            return;
        }

        int damage = player.Tank.Health + player.Tank.Shield;

        player.Tank.TakeDamage(damage);

        Console.WriteLine($"Player {_playerId} killed.");
    }
}
