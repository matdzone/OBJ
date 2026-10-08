public class TankControlsAdapter : ITankControls
{
	private readonly GameSession game;
	private readonly int playerId;

	public TankControlsAdapter(GameSession game, int playerId)
	{
		this.game = game;
		this.playerId = playerId;
	}

	public void SetMovement(int drive, int turn)
	{
		game.SetInput(playerId, drive, turn);
	}

	public void ChangeShell(string movement)
	{
		game.SetShellMovement(playerId, movement);
	}

	public string GetShellMovement()
	{
		Player? player = game.GetPlayer(playerId);

		return player?.Tank.ShellMovement ?? "Straight";
	}

	public void Shoot()
	{
		game.Shoot(playerId);
	}
}