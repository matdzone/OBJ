using System.Net.Http;
using System.Text.Json;
using var client = new HttpClient();

int playerId = 0;
Console.WriteLine("press w a s d to move");

while(true)
{
    var key = Console.ReadKey(true).Key;
    int x = 0;
    int y = 0;

    var response = await client.GetAsync($"http://localhost:5062/players");
    string playersContent = await response.Content.ReadAsStringAsync();


    using JsonDocument json = JsonDocument.Parse(playersContent);
    


    //Console.WriteLine(playersContent);
    Console.Clear();


 foreach (JsonElement player in json.RootElement.GetProperty("players").EnumerateArray())
    {
        string name = player.GetProperty("name").GetString()!;
        int playerX = player.GetProperty("x").GetInt32();
        int playerY = player.GetProperty("y").GetInt32();

        // 8. Nueinam į žaidėjo X/Y vietą
        Console.SetCursorPosition(playerX, playerY);

        // 9. Nupiešiam pirmą jo vardo raidę
        Console.Write(name[0]);
    }
    switch(key)
    {
        case ConsoleKey.W:
        y = -1;
        break;
    case ConsoleKey.A:
        x = -1;
        break;
    case ConsoleKey.S:
        y = 1;
        break;
    case ConsoleKey.D:
        x = 1;
        break;
    }
    await client.PostAsync($"http://localhost:5062/move/{playerId}/{x}/{y}",null);

//string content = await response.Content.ReadAsStringAsync();
//Console.WriteLine(content);

//using JsonDocument json = JsonDocument.Parse(content);

//int playerX = json.RootElement.GetProperty("x").GetInt32();
//int playerY = json.RootElement.GetProperty("y").GetInt32();
///Console.WriteLine($"Player position: ({playerX}, {playerY})");

//Console.Clear();
//Console.SetCursorPosition(playerX, playerY);
//Console.Write("T");
}