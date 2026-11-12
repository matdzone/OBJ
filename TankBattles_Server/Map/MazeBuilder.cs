public class MazeBuilder : MazeBuilderBase
{
    private readonly WallTypeFactory _wallTypeFactory = new();

    public Maze Build()
    {
        if (Walls.Any(w => w.X < 0 || w.X >= Width || w.Y < 0 || w.Y >= Height))
            throw new InvalidOperationException("A wall is outside the map.");

        if (Walls.Contains((2, 2)))
            throw new InvalidOperationException("Spawn tile (2,2) is blocked.");

        // Flyweight: gauname bendrą sienos tipo objektą
        WallType stone = _wallTypeFactory.GetWallType("Stone", false);

        // Visos sienos naudoja tą patį WallType
        Maze maze = new(
            Walls.Select(w => new Wall(w.X, w.Y, stone)).ToList(),
            Width,
            Height
        );
        if (maze.Walls.Count >= 2)
{
    
}
        int openTiles = Width * Height - Walls.Count;
        int reachable = PathFinder.Distances(maze, new Position(2, 2)).Count;

        if (reachable != openTiles)
            throw new InvalidOperationException("Map has unreachable areas.");

        return maze;
    }
}