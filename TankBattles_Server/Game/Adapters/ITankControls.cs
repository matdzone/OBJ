public interface ITankControls
{
	void SetMovement(int drive, int turn);
	void ChangeShell(string movement);
	string GetShellMovement();
	void Shoot();
}