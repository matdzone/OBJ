using System.Net.Http;
using System.Text.Json;

using var client = new HttpClient();

Console.WriteLine("Enter your name:");
string playername = Console.ReadLine()!;

var res = await client.PostAsync(
	$"http://localhost:5062/join/{playername}",
	null
);

string playersID = await res.Content.ReadAsStringAsync();

using JsonDocument json1 = JsonDocument.Parse(playersID);

int playerId =
	json1.RootElement.GetProperty("id").GetInt32();

Console.WriteLine("press w a s d to move");

while (true)
{
	int x = 0;
	int y = 0;

	if (Console.KeyAvailable)
	{
		ConsoleKeyInfo keyInfo =
			Console.ReadKey(intercept: true);

		if (keyInfo.Key == ConsoleKey.W) { y = -1; }
		if (keyInfo.Key == ConsoleKey.D) { x = 1; }
		if (keyInfo.Key == ConsoleKey.A) { x = -1; }
		if (keyInfo.Key == ConsoleKey.S) { y = 1; }

		if (keyInfo.Key == ConsoleKey.Q)
		{
			return;
		}

		if (x != 0 || y != 0)
		{
			await client.PostAsync(
				$"http://localhost:5062/move/{playerId}/{x}/{y}",
				null
			);
		}
	}

	var response =
		await client.GetAsync(
			"http://localhost:5062/players"
		);

	string playersContent =
		await response.Content.ReadAsStringAsync();

	using JsonDocument json =
		JsonDocument.Parse(playersContent);

	Console.Clear();

	foreach (
		JsonElement player
		in json.RootElement
			.GetProperty("players")
			.EnumerateArray())
	{
		string name =
			player.GetProperty("name").GetString()!;

		int playerX =
			player.GetProperty("x").GetInt32();

		int playerY =
			player.GetProperty("y").GetInt32();

		if (playerX >= 0 &&
			playerY >= 0 &&
			playerX < Console.BufferWidth &&
			playerY < Console.BufferHeight)
		{
			Console.SetCursorPosition(playerX, playerY);
			Console.Write(name[0]);
		}
	}

	await Task.Delay(10);
}