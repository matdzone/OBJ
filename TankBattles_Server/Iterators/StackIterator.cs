public class StackIterator<T> : IIterator<T>
{
	private readonly T[] _snapshot;
	private int _index;

	public StackIterator(Stack<T> stack)
	{
		_snapshot = stack.ToArray();
	}

	public bool HasNext()
	{
		return _index < _snapshot.Length;
	}

	public T Next()
	{
		if (!HasNext())
			throw new InvalidOperationException("No more elements.");

		return _snapshot[_index++];
	}

	public void Reset()
	{
		_index = 0;
	}
}
