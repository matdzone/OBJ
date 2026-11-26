
public class ConsoleInterpreter
{
    private readonly CommandParser _parser;
    private readonly InterpreterContext _context;

    public ConsoleInterpreter(GameSession game)
    {
        _parser = new CommandParser();
        _context = new InterpreterContext(game);
    }

    public void Run()
    {
        Console.WriteLine("Console interpreter started.");

        while (true)
        {
            string? input = Console.ReadLine();

            if (input == null)
            {
                break;
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            try
            {
                IExpression expression = _parser.Parse(input);

                _context.Game.ExecuteAdminAction(
    () => expression.Interpret(_context)
);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
