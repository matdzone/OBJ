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
		if (Players.Any(p => p.ID == player.ID))
			return;

		Players.Add(player);

		if (HostId == null)
			HostId = player.ID;
	}

	public void RemovePlayer(int playerId)
	{
		Players.RemoveAll(
			p => p.ID == playerId
		);

		if (HostId == playerId)
		{
			if (Players.Count > 0)
				HostId = Players[0].ID;
			else
				HostId = null;
		}
	}

	public void SetGameMode(string gameMode)
	{
		GameMode = gameMode;
	}

	public int GetRequiredPlayers()
	{
		if (GameMode == "Practice")
			return 1;

		return 2;
	}

	public bool CanStartGame()
	{
		if (Players.Count < GetRequiredPlayers())
			return false;

		foreach (Player player in Players)
		{
			if (!player.IsReady)
				return false;
		}

		return true;
	}

	public bool StartGame(int playerId)
	{
		if (HostId != playerId)
			return false;

		if (!CanStartGame())
			return false;

		IsStarted = true;

		return true;
	}
}