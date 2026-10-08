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
GameFacade facade = new GameFacade(gameManager);
CommandInvoker commandInvoker = new CommandInvoker();
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

app.MapPost("/join/{name}/{tankType}", (string name, string tankType) =>
{

	Player player = facade.JoinGame(name, tankType);
	lobby.AddPlayer(player);

	return new
	{
		message = "joined " + name,
		name = player.Name,
		id = player.ID
	};
});
app.MapPost("/lobby/mode/{playerId}/{gameMode}", (int playerId, string gameMode) =>
{
	if (!facade.SetGameMode(playerId, gameMode))
		return Results.BadRequest(new { message = "Cannot change game mode" });

	return Results.Ok(new
	{
		message = "Game mode changed",
		gameMode = lobby.GameMode
	});
});
app.MapGet("/lobby", () =>
{
	Lobby lobby =
		GameManager.Instance.Lobby;

	return Results.Ok(new
	{
		name = lobby.Name,
		hostId = lobby.HostId,
		gameMode = lobby.GameMode,
		isStarted = lobby.IsStarted,
		requiredPlayers = lobby.GetRequiredPlayers(),
		canStart = lobby.CanStartGame(),
		players = lobby.Players
	});
});
app.MapPost("/lobby/start/{playerId}", (int playerId) =>
{
	bool started = facade.StartGame(playerId);


	if (!started)
		return Results.BadRequest(new { message = "Game cannot be started" });

	return Results.Ok(new { message = "Game started" });
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
		Player? player = game.GetPlayer(id);

		if (player == null || !Tank.ShellMovements.Contains(movement))
			return Results.BadRequest(new { message = "unknown player or movement" });

		ITankControls controls =
			new TankControlsAdapter(game, id);

		IGameCommand command =
			new ChangeShellCommand(controls, movement);

		commandInvoker.Execute(id, command);

		return Results.Ok(new
		{
			id,
			movement
		});
	});

app.MapPost("/undo/{id}", (int id) =>
{
	if (!commandInvoker.Undo(id))
		return Results.BadRequest(new { message = "nothing to undo" });

	Player? player = game.GetPlayer(id);

	return Results.Ok(new
	{
		message = "command undone",
		movement = player?.Tank.ShellMovement
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

app.MapPost("/split/{id}", (int id) =>
{
	bool projectile = game.SplitProjectiles(id, 3, 60);

	if (!projectile)
	{
		return Results.NotFound(new
		{
			message = "player does not exist or doesnt have any bullets on map"
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
		if (!facade.SetPlayerReady(id, ready))
			return Results.NotFound();

		return Results.Ok(game.GetPlayer(id));
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