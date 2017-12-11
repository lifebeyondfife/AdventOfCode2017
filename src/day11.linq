<Query Kind="Program" />

void Main()
{
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day11.txt")))
	{
		var steps = GetSteps(file);

		var route = GetRoute(steps).ToList();
		var origin = new Coordinate(0, 0);
		
		// Part 1
		route.Last().ManhattanDistance(origin).Dump();
		
		// Part 2
		route.Select(c => c.ManhattanDistance(origin)).Max().Dump();	
	}
}

// Define other methods and classes here
IList<Direction> GetSteps(StreamReader file)
{
	return file.ReadLine().Split(',').Select(s => (Direction) Enum.Parse(typeof(Direction), s)).ToList();
}

public enum Direction
{
	n,
	ne,
	se,
	s,
	sw,
	nw
}

public struct Coordinate
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
			case Direction.n: return new Coordinate(X, Y + 2);
			case Direction.s: return new Coordinate(X, Y - 2);
			case Direction.nw: return new Coordinate(X - 1, Y + 1);
			case Direction.ne: return new Coordinate(X + 1, Y + 1);
			case Direction.sw: return new Coordinate(X - 1, Y - 1);
			case Direction.se: return new Coordinate(X + 1, Y - 1);
		}
		
		throw new ApplicationException("Unknown direction.");
	}
	
	public int ManhattanDistance(Coordinate a)
	{
		var steps = Math.Abs(X - a.X);
		
		if (Math.Abs(Y - a.Y) > steps)
		{
			steps += (Math.Abs(Y - a.Y) - steps) / 2;
		}
		
		return steps;
	}
}

public IEnumerable<Coordinate> GetRoute(IList<Direction> steps)
{
	var origin = new Coordinate(0, 0);

	return steps.Select(s =>
		{
			var advance = origin.Apply(s);
			origin = advance;
			return advance;
		});
}
