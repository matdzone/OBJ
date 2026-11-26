
public class HealExpression : IExpression
{
    private readonly int _playerId;
    private readonly int _amount;

    public HealExpression(int playerId, int amount)
    {
        _playerId = playerId;
        _amount = amount;
    }

    public void Interpret(InterpreterContext context)
    {
        Player? player = context.Game.GetPlayer(_playerId);

        if (player == null)
        {
            Console.WriteLine("Player not found.");
            return;
        }

        if (_amount <= 0)
        {
            Console.WriteLine("Heal amount must be positive.");
            return;
        }

        if (!player.Tank.IsAlive)
        {
            Console.WriteLine("Player is dead.");
            return;
        }

        player.Tank.Heal(_amount);

        Console.WriteLine(
            $"Player {_playerId} health: " +
            $"{player.Tank.Health}/{player.Tank.MaxHealth}"
        );
    }
}
