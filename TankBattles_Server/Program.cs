var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

//Iki singleton
/*
GameSession game = new GameSession();

Lobby lobby = new Lobby("Main Lobby");
*/

GameManager gameManager = GameManager.Instance;

GameSession game = gameManager.GameSession;
Lobby lobby = gameManager.Lobby;

app.MapGet("/api", () =>
{
	return "tank battles server!";
});

app.MapGet("/status", () =>
{
	return new
	{
		message = "Tank Battles server is running",
		playerCount = game.Players.Count
	};
});

app.MapGet("/players", () =>
{
	return new
	{
		message = "all players",
		players = game.Players
	};
});

app.MapPost("/join/{name}", (string name) =>
{
	Player player = game.AddPlayer(name);

	lobby.AddPlayer(player);

	return new
	{
		message = "joined " + name,
		name = player.Name,
		id = player.ID
	};
});

app.MapPost(
	"/move/{id}/{deltaX}/{deltaY}",
	(int id, int deltaX, int deltaY) =>
	{
		Player? player = game.GetPlayer(id);

		if (player == null)
		{
			return Results.NotFound(new
			{
				message = "player " + id + " does not exist"
			});
		}

		bool moved = game.MovePlayer(id, deltaX, deltaY);

		return Results.Ok(new
		{
			message = moved ? "moved player " + id : "movement blocked",
			id,
			x = player.X,
			y = player.Y
		});
});

app.MapPost("/shoot/{id}", (int id) =>
{
	Projectile? projectile = game.Shoot(id);

	if (projectile == null)
	{
		return Results.NotFound(new
		{
			message = "player does not exist or tank is destroyed"
		});
	}

	return Results.Ok(projectile);
});

app.MapGet("/projectiles", () =>
{
	return game.Projectiles;
});

app.MapGet("/maze", () =>
{
	return game.Maze;
});

app.MapPost(
	"/ready/{id}/{ready}",
	(int id, bool ready) =>
	{
		Player? player = game.GetPlayer(id);

		if (player == null)
			return Results.NotFound();

		player.SetReady(ready);

		return Results.Ok(player);
});

_ = Task.Run(async () =>
{
	using PeriodicTimer timer =
		new PeriodicTimer(TimeSpan.FromMilliseconds(100));

	while (await timer.WaitForNextTickAsync())
	{
		game.UpdateProjectiles();
	}
});

app.Run();