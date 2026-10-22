public static class MazeDirector
{


    public static T Classic<T>(T b) where T : MazeBuilderBase
    {
        b.WithBorder()
        .WithVerticalWall(x: 10, fromY: 4, toY: 9)
        .WithHorizontalWall(y: 14, fromX: 16, toX: 23)
        .WithVerticalWall(x: 29, fromY: 6, toY: 12);

        return b;
    }
    
    public static T Arena<T>(T b) where T : MazeBuilderBase
    {
        b.WithBorder();

        return b;
    }

    public static T Cross<T>(T b) where T : MazeBuilderBase
    {
        b.WithBorder()
        .WithHorizontalWall(y: 10, fromX: 8, toX: 31)
        .WithVerticalWall(x: 20, fromY: 3, toY: 16);

        return b;
    }
}