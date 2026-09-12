using System.Net.Http;
using var client = new HttpClient();

int playerId = 0;
Console.WriteLine("press w a s d to move");

while(true)
{
    var key = Console.ReadKey(true).Key;
    int x = 0;
    int y = 0;
    switch(key)
    {
        case ConsoleKey.W:
            y=+1;
            break;
        case ConsoleKey.A:
            x=-1;
            break;
        case ConsoleKey.S:
            y=-1;
            break;
        case ConsoleKey.D:
            x=+1;
            break;
    }
    var response = client.PostAsync(
        $"http://localhost:5062/move/{playerId}/{x}/{y}", null);
    Console.WriteLine(response.Result.Content.ReadAsStringAsync().Result);
}