public class MazeBuilderBase
{
    protected readonly int Width;
    protected readonly int Height;
    protected readonly HashSet<(int X, int Y)> Walls = new();

    public MazeBuilderBase(int width = 40, int height = 20)
    {
        Width = width;
        Height = height;
    }

    public MazeBuilderBase WithWall(int x, int y)
    {
        Walls.Add((x, y));
        return this;
    }

    public MazeBuilderBase WithBorder()
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

    public MazeBuilderBase WithHorizontalWall(int y, int fromX, int toX)
    {
        for (int x = fromX; x <= toX; x++)
            Walls.Add((x, y));

        return this;
    }

    public MazeBuilderBase WithVerticalWall(int x, int fromY, int toY)
    {
        for (int y = fromY; y <= toY; y++)
            Walls.Add((x, y));

        return this;
    }
}