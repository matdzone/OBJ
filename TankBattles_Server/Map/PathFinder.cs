public static class PathFinder
{
	private static readonly (int DeltaX, int DeltaY)[] Directions =
	{
		(0, -1), (1, 0), (0, 1), (-1, 0),
		(1, -1), (1, 1), (-1, 1), (-1, -1)
	};

	public static (int DeltaX, int DeltaY)? NextStep(
		Maze maze,
		Position from,
		Position to)
	{
		(int X, int Y) start = (from.X, from.Y);
		(int X, int Y) goal = (to.X, to.Y);

		if (start == goal)
			return null;

		Dictionary<(int X, int Y), (int DeltaX, int DeltaY)> firstStep = new();
		HashSet<(int X, int Y)> visited = new() { start };
		Queue<(int X, int Y)> queue = new();

		queue.Enqueue(start);

		while (queue.Count > 0)
		{
			(int X, int Y) current = queue.Dequeue();

			foreach ((int dx, int dy) in Directions)
			{
				if (!CanStep(maze, current.X, current.Y, dx, dy))
					continue;

				(int X, int Y) next = (current.X + dx, current.Y + dy);

				if (!visited.Add(next))
					continue;

				(int DeltaX, int DeltaY) step =
					current == start ? (dx, dy) : firstStep[current];

				if (next == goal)
					return step;

				firstStep[next] = step;
				queue.Enqueue(next);
			}
		}

		return null;
	}

	public static Dictionary<(int X, int Y), int> Distances(
		Maze maze,
		Position from)
	{
		(int X, int Y) start = (from.X, from.Y);

		Dictionary<(int X, int Y), int> distances = new() { [start] = 0 };
		Queue<(int X, int Y)> queue = new();

		queue.Enqueue(start);

		while (queue.Count > 0)
		{
			(int X, int Y) current = queue.Dequeue();

			foreach ((int dx, int dy) in Directions)
			{
				if (!CanStep(maze, current.X, current.Y, dx, dy))
					continue;

				(int X, int Y) next = (current.X + dx, current.Y + dy);

				if (distances.ContainsKey(next))
					continue;

				distances[next] = distances[current] + 1;
				queue.Enqueue(next);
			}
		}

		return distances;
	}

	private static bool CanStep(Maze maze, int x, int y, int dx, int dy)
	{
		if (!maze.CanMoveTo(x + dx, y + dy))
			return false;

		if (dx != 0 && dy != 0)
		{
			return maze.CanMoveTo(x + dx, y) &&
				maze.CanMoveTo(x, y + dy);
		}

		return true;
	}
}
