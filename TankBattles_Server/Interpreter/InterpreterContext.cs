public class InterpreterContext
{
    public GameSession Game { get; }

    public InterpreterContext(GameSession game)
    {
        Game = game;
    }
}
