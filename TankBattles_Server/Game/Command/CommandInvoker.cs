public class CommandInvoker
{
	private readonly Dictionary<int, Stack<IGameCommand>> history = new();

	public void Execute(int playerId, IGameCommand command)
	{
		command.Execute();

		if (!history.ContainsKey(playerId))
			history[playerId] = new Stack<IGameCommand>();

		history[playerId].Push(command);
	}

	public IIterator<IGameCommand> CreateHistoryIterator(int playerId)
	{
		if (!history.ContainsKey(playerId))
			return new StackIterator<IGameCommand>(new Stack<IGameCommand>());

		return new StackIterator<IGameCommand>(history[playerId]);
	}

	public bool Undo(int playerId)
	{
		if (!history.ContainsKey(playerId) || history[playerId].Count == 0)
			return false;

		IGameCommand command = history[playerId].Pop();
		command.Undo();

		return true;
	}
}