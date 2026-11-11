public interface IGameCommand
{
	void Execute();
	void Undo();
	string Description { get; }
}