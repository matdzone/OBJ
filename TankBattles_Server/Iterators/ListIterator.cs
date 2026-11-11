public class ListIterator<T> : IIterator<T>
{
	private readonly List<T> _items;
	private int _index;

	public ListIterator(List<T> items)
	{
		_items = items;
	}

	public bool HasNext()
	{
		return _index < _items.Count;
	}

	public T Next()
	{
		if (!HasNext())
			throw new InvalidOperationException("No more elements.");

		return _items[_index++];
	}

	public void Reset()
	{
		_index = 0;
	}
}
