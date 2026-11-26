
public class ShieldExpression : IExpression
{
    private readonly int _playerId;

    public ShieldExpression(int playerId)
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
            Console.WriteLine("Player is dead.");
            return;
        }

        if (player.Tank.Shield > 0)
        {
            Console.WriteLine("Player already has a shield.");
            return;
        }

        player.Tank.AddShield(ShieldBox.ShieldAmount);

        Console.WriteLine(
            $"Player {_playerId} shield: {player.Tank.Shield}"
        );
    }
}
