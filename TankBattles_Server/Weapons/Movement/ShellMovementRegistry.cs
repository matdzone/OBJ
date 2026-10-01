public class ShellMovementRegistry
{
    private Dictionary<string, IMovementStrategy> _prototypes;

    public ShellMovementRegistry(Maze maze)
    {
        _prototypes = new()
        {
            ["Straight"]     = new StraightMovement(),
            ["Bouncing"]     = new BouncingMovement(maze),
            ["Accelerating"] = new AcceleratingMovement(),
        };
    }

    public IMovementStrategy Create(string name) => _prototypes[name].Clone();
}