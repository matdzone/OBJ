public sealed class GameManager{
	private static readonly Lazy<GameManager> instance = new(() => new GameManager());

	public static GameManager Instance {
		get { return instance.Value; } 
	}

	public GameSession GameSession { get; }
	public Lobby Lobby { get; }

	private GameManager() 
	{
		GameSession = new GameSession();
		Lobby = new Lobby("Main Lobby");
	}


}