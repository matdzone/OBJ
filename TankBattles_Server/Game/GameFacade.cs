public class GameFacade
{
    private readonly GameSession game;
    private readonly Lobby lobby;

    public GameFacade(GameManager manager)
    {
        game = manager.GameSession;
        lobby = manager.Lobby;
    }

    public Player JoinGame(string name, string tankType)
    {
        ITankFactory factory;

        if (tankType == "light")
        {
            factory = new LightTankFactory();
        }
        else
        {
            factory = new HeavyTankFactory();
        }

        Player player = game.AddPlayer(name, factory);

        lobby.AddPlayer(player);

        return player;
    }
    public bool StartGame(int playerId)
    {
        return lobby.StartGame(playerId);
    }
    public bool SetPlayerReady(int playerId, bool ready)
    {
        Player? player = game.GetPlayer(playerId);

        if (player == null)
            return false;

        player.SetReady(ready);

        return true;
    }
    public bool SetGameMode(int playerId, string gameMode)
    {
        if (lobby.HostId != playerId)
            return false;

        if (gameMode != "Deathmatch" && gameMode != "Practice")
            return false;

        lobby.SetGameMode(gameMode);

        return true;
    }
}