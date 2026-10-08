public class ChangeShellCommand : IGameCommand
{
	private readonly ITankControls controls;
	private readonly string newMovement;
	private string oldMovement = "";

	public ChangeShellCommand(ITankControls controls, string movement)
	{
		this.controls = controls;
		newMovement = movement;
	}

	public void Execute()
	{
		oldMovement = controls.GetShellMovement();
		controls.ChangeShell(newMovement);
	}

	public void Undo()
	{
		controls.ChangeShell(oldMovement);
	}
}