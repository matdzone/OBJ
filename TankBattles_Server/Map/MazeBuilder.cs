public class MazeBuilder : MazeBuilderBase
{

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