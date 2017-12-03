<Query Kind="Program" />

void Main()
{
	var accessPort = new Coordinate(0, 0);
	SpiralMemory.Memory[accessPort] = 1;
		
	var generateGrid = new [] { Tuple.Create(accessPort, 1) }.Concat(NextDirection().SelectMany(sd => LineOfCoordinates(sd)));
	
	// Part 1
	// Func<int, Tuple<Coordinate, int>> nthElement = n => generateGrid.Skip(n - 1).First();
	// ManhattanDistance(accessPort, nthElement(265149).Item1).Dump();

	// Part 2
	var values = generateGrid.TakeWhile(coordValue => coordValue.Item2 < 265149).ToList();
	
	SpiralMemory.Memory.Values.Max().Dump();
}

// Define other methods and classes here
enum Direction
{
	Right,
	Up,
	Left,
	Down
}

struct Coordinate
{
	public int X, Y;
	
	public Coordinate(int x, int y)
	{
		X = x;
		Y = y;
	}
	
	public Coordinate Apply(Direction direction)
	{
		switch (direction)
		{
			case Direction.Up: return new Coordinate(X, Y + 1);
			case Direction.Down: return new Coordinate(X, Y - 1);
			case Direction.Left: return new Coordinate(X - 1, Y);
			case Direction.Right: return new Coordinate(X + 1, Y);
		}
		
		throw new ApplicationException("Unknown direction.");
	}
}

IEnumerable<Tuple<int, Direction>> NextDirection()
{
	var steps = 1;
	while (true)
	{
		yield return Tuple.Create(steps, Direction.Right);
		yield return Tuple.Create(steps++, Direction.Up);
		yield return Tuple.Create(steps, Direction.Left);
		yield return Tuple.Create(steps++, Direction.Down);
	}
}

static class SpiralMemory
{
	internal static Coordinate CurrentCoordinate = new Coordinate(0, 0);

	internal static Dictionary<Coordinate, int> Memory = new Dictionary<Coordinate, int>();
}

IEnumerable<Tuple<Coordinate, int>> LineOfCoordinates(Tuple<int, Direction> stepDirection)
{
	for (int i = 0; stepDirection.Item1 - i > 0; ++i)
	{
		SpiralMemory.CurrentCoordinate = SpiralMemory.CurrentCoordinate.Apply(stepDirection.Item2);
		
		yield return Tuple.Create(SpiralMemory.CurrentCoordinate, UpdateMemory(SpiralMemory.CurrentCoordinate));
	}
}

int ManhattanDistance(Coordinate a, Coordinate b)
{
	return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
}

int UpdateMemory(Coordinate coordinate)
{
	var minusOnetoOne = Enumerable.Range(-1, 3);
	
	SpiralMemory.Memory[coordinate] = minusOnetoOne.
		SelectMany(x =>
			minusOnetoOne.Select(y => new Coordinate(coordinate.X + x, coordinate.Y + y))
		).
		Where(c => SpiralMemory.Memory.ContainsKey(c)).
		Select(c => SpiralMemory.Memory[c]).
		Sum();
	
	return SpiralMemory.Memory[coordinate];
}

