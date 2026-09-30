public class MazeBuilder
{
    private int Width;
    private int Height;
    private HashSet<(int X, int Y)> Walls = new();

    public MazeBuilder(int width = 40, int height = 20)
    {
        Width = width;
        Height = height;
    }

    public MazeBuilder WithWall(int x, int y)
    {
        Walls.Add((x, y));
        return this;
    }

    public MazeBuilder WithBorder()
    {
        for (int x = 0; x < Width; x++)
        {
            Walls.Add((x, 0));
            Walls.Add((x, Height - 1));
        }

        for (int y = 1; y < Height - 1; y++)
        {
            Walls.Add((0, y));
            Walls.Add((Width - 1, y));
        }

        return this;
    }

    public MazeBuilder WithHorizontalWall(int y, int fromX, int toX)
    {
        for (int x = fromX; x <= toX; x++)
            Walls.Add((x, y));

        return this;
    }

    public MazeBuilder WithVerticalWall(int x, int fromY, int toY)
    {
        for (int y = fromY; y <= toY; y++)
            Walls.Add((x, y));

        return this;
    }

    public Maze Build()
    {
        if (Walls.Any(w => w.X < 0 || w.X >= Width || w.Y < 0 || w.Y >= Height))
            throw new InvalidOperationException("A wall is outside the map.");

        if (Walls.Contains((2, 2)))
            throw new InvalidOperationException("Spawn tile (2,2) is blocked.");

        Maze maze = new(Walls.Select(w => new Wall(w.X, w.Y)).ToList(), Width, Height);

        int openTiles = Width * Height - Walls.Count;
        int reachable = PathFinder.Distances(maze, new Position(2, 2)).Count;

        if (reachable != openTiles)
            throw new InvalidOperationException("Map has unreachable areas.");

        return maze;
    }
}