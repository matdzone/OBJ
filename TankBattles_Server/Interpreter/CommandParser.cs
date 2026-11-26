
public class CommandParser
{
    public IExpression Parse(string input)
    {
        string[] commands = input.Split(
            ';', StringSplitOptions.RemoveEmptyEntries
        );

        List<IExpression> expressions = new();

        foreach (string command in commands)
        {
            expressions.Add(ParseCommand(command));
        }

        if (expressions.Count == 0)
        {
            throw new FormatException("No command entered.");
        }

        if (expressions.Count == 1)
        {
            return expressions[0];
        }

        return new SequenceExpression(expressions);
    }

    private IExpression ParseCommand(string command)
    {
        string[] parts = command.Split(
            ' ', StringSplitOptions.RemoveEmptyEntries
        );

        if (parts.Length == 0)
        {
            throw new FormatException("Empty command.");
        }

        switch (parts[0].ToUpperInvariant())
        {
            case "HEAL" when parts.Length == 3:
                return new HealExpression(
                    int.Parse(parts[1]),
                    int.Parse(parts[2])
                );

            case "SHIELD" when parts.Length == 2:
                return new ShieldExpression(
                    int.Parse(parts[1])
                );

            case "KILL" when parts.Length == 2:
                return new KillExpression(
                    int.Parse(parts[1])
                );

            case "TELEPORT" when parts.Length == 4:
                return new TeleportExpression(
                    int.Parse(parts[1]),
                    int.Parse(parts[2]),
                    int.Parse(parts[3])
                );

            default:
                throw new FormatException(
                    $"Invalid command: {command}"
                );
        }
    }
}
