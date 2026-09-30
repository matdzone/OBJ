public static class MazeDirector
{
    public static Maze Classic() => new MazeBuilder()
        .WithBorder()
        .WithVerticalWall(x: 10, fromY: 4, toY: 9)
        .WithHorizontalWall(y: 14, fromX: 16, toX: 23)
        .WithVerticalWall(x: 29, fromY: 6, toY: 12)
        .Build();

    public static Maze Arena() => new MazeBuilder()
        .WithBorder()
        .Build();

    public static Maze Cross() => new MazeBuilder()
        .WithBorder()
        .WithHorizontalWall(y: 10, fromX: 8, toX: 31)
        .WithVerticalWall(x: 20, fromY: 3, toY: 16)
        .Build();
}