using System.Text;
public class MazeBuilderString : MazeBuilderBase
{
    public string Build()
    {
        var sb = new StringBuilder();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
                sb.Append(Walls.Contains((x, y)) ? '#' : ' ');
            sb.AppendLine();
        }
        return sb.ToString();
    }
}