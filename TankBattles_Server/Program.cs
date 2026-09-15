var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List <Player> players = new List<Player>();

app.MapGet("/", () => 
{
    return "tank battles server!";
}

);

app.MapGet("/status", () =>
{
    return new
    {
        message = "Tank Battles server is running",
        players.Count
    };
});

app.MapGet("/players",() =>
{
    return new
    {
        message = "all players",
        players
    };
});

app.MapPost("/join/{name}", (string name) => 
{
    Player player = new Player(players.Count, name);
    players.Add(player);
    return new
    {
        message = "joined " + name,
        players.Last().Name,
        players.Last().ID
    };
});

app.MapPost("/move/{id}/{deltaX}/{deltaY}", (int id, int deltaX, int deltaY) =>
{
    if (id >= 0 && id < players.Count)
    {
        players[id].Move(deltaX, deltaY);

        return new
        {
            message = "moved player " + id,
            ID = (int?)id,
            X = (int?)players[id].X,
            Y = (int?)players[id].Y
        };
    }

    return new
    {
        message = "player " + id + " does not exist",
        ID = (int?)null,
        X = (int?)null,
        Y = (int?)null
    };
});
app.Run();
