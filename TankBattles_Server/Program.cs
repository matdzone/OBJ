var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

GameSession game = new GameSession();

Lobby lobby = new Lobby("Main Lobby");

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

app.MapPost(
	"/input/{id}/{drive}/{turn}",
	(int id, int drive, int turn) =>
	{
		if (!game.SetInput(id, drive, turn))
			return Results.NotFound(new { message = "player " + id + " does not exist" });

		return Results.Ok();
});

app.MapPost(
	"/shell/{id}/{movement}",
	(int id, string movement) =>
	{
		if (!game.SetShellMovement(id, movement))
			return Results.BadRequest(new { message = "unknown player or movement" });

		return Results.Ok(new { id, movement });
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

app.MapGet("/boxes", () =>
{
	return game.Boxes;
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
		new PeriodicTimer(TimeSpan.FromMilliseconds(30));

	System.Diagnostics.Stopwatch stopwatch =
		System.Diagnostics.Stopwatch.StartNew();

	while (await timer.WaitForNextTickAsync())
	{
		double deltaSeconds = stopwatch.Elapsed.TotalSeconds;

		stopwatch.Restart();

		game.Update(Math.Min(deltaSeconds, 0.1));
	}
});

_ = Task.Run(async () =>
{
	using PeriodicTimer timer =
		new PeriodicTimer(TimeSpan.FromSeconds(10));

	while (await timer.WaitForNextTickAsync())
	{
		game.SpawnBox();
	}
});

app.Run();