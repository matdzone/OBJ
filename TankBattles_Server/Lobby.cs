public class Lobby
{
	public string Name { get; }
	public List<Player> Players { get; } = new();

	public int? HostId { get; private set; }

	public string GameMode { get; private set; } = "Deathmatch";

	public bool IsStarted { get; private set; }

	public Lobby(string name)
	{
		Name = name;
	}

	public void AddPlayer(Player player)
	{
		if (!Players.Any(p => p.ID == player.ID))
			Players.Add(player);
	}

	public void RemovePlayer(int playerId)
	{
		Players.RemoveAll(p => p.ID == playerId);
	}

	public void SetHost(int playerId)
	{
		HostId = playerId;
	}

	public void SetGameMode(string gameMode)
	{
		GameMode = gameMode;
	}

	public void StartGame()
	{
		IsStarted = true;
	}
}