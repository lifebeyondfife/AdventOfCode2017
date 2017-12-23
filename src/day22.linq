<Query Kind="Program" />

void Main()
{
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day22.txt")))
	{
		var grid = GetGridStream(file);
		
		var burstIterations = 10000;
		int causedInfectionCount;
		
		SimulateSimple(grid, burstIterations, out causedInfectionCount);
		
		// Part 1
		causedInfectionCount.Dump();		
	}

	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day22.txt")))
	{
		var grid = GetGridStream(file);
		
		var burstIterations = 10000000;
		int causedInfectionCount;
		
		SimulateComplex(grid, burstIterations, out causedInfectionCount);
		
		// Part 2
		causedInfectionCount.Dump();		
	}
}

// Define other methods and classes here
IDictionary<Coordinate, Virus> GetGridStream(StreamReader file)
{
	var grid = new Dictionary<Coordinate, Virus>();
	var y = -12;
	
	while (!file.EndOfStream)
	{
		var line = file.ReadLine();
		for (var x = -12; x <= 12; ++x)
			if (line[x + 12] == '#')
				grid[new Coordinate(x, y)] = Virus.Infected;
		
		++y;
	}
	
	return grid;
}

public enum Direction
{
	up,
	down,
	left,
	right
}

public struct Coordinate
{
	public int X;
	public int Y;
	
	public Coordinate(int x, int y)
	{
		X = x;
		Y = y;
	}
}

public enum Virus
{
	Clean,
	Weakened,
	Infected,
	Flagged
}

public void SimulateSimple(IDictionary<Coordinate, Virus> grid, int burstIterations, out int causedInfectionCount)
{
	causedInfectionCount = 0;
	var coordinate = new Coordinate(0, 0);
	var direction = Direction.up;
	
	Func<Direction, Direction> turnLeft = dir =>
	{
		switch (dir)
		{
			case Direction.up: return Direction.left;
			case Direction.left: return Direction.down;
			case Direction.down: return Direction.right;
			case Direction.right: return Direction.up;
		}
		
		throw new Exception("Unknown direction.");	
	};
	
	Func<Direction, Direction> turnRight = dir =>
	{
		switch (dir)
		{
			case Direction.up: return Direction.right;
			case Direction.left: return Direction.up;
			case Direction.down: return Direction.left;
			case Direction.right: return Direction.down;
		}
		
		throw new Exception("Unknown direction.");	
	};
	
	Action<Direction> advance = dir =>
	{
		switch (dir)
		{
			case Direction.up: --coordinate.Y; break;
			case Direction.left: --coordinate.X; break;
			case Direction.down: ++coordinate.Y; break;
			case Direction.right: ++coordinate.X; break;
		}
	};
	
	while (--burstIterations >= 0)
	{
		direction = grid.ContainsKey(coordinate) ? turnRight(direction) : turnLeft(direction);
		
		if (!grid.ContainsKey(coordinate))
		{
			++causedInfectionCount;
			grid[coordinate] = Virus.Infected;
		}
		else
			grid.Remove(coordinate);
		
		advance(direction);
	}
}

public void SimulateComplex(IDictionary<Coordinate, Virus> grid, int burstIterations, out int causedInfectionCount)
{
	causedInfectionCount = 0;
	var coordinate = new Coordinate(0, 0);
	var direction = Direction.up;
	
	Func<Direction, Direction> turnLeft = dir =>
	{
		switch (dir)
		{
			case Direction.up: return Direction.left;
			case Direction.left: return Direction.down;
			case Direction.down: return Direction.right;
			case Direction.right: return Direction.up;
		}
		
		throw new Exception("Unknown direction.");	
	};
	
	Func<Direction, Direction> turnRight = dir =>
	{
		switch (dir)
		{
			case Direction.up: return Direction.right;
			case Direction.left: return Direction.up;
			case Direction.down: return Direction.left;
			case Direction.right: return Direction.down;
		}
		
		throw new Exception("Unknown direction.");	
	};
	
	Action<Direction> advance = dir =>
	{
		switch (dir)
		{
			case Direction.up: --coordinate.Y; break;
			case Direction.left: --coordinate.X; break;
			case Direction.down: ++coordinate.Y; break;
			case Direction.right: ++coordinate.X; break;
		}
	};
	
	while (--burstIterations >= 0)
	{
		if (!grid.ContainsKey(coordinate) || grid[coordinate] == Virus.Clean)
		{
			direction = turnLeft(direction);
			grid[coordinate] = Virus.Weakened;
			advance(direction);
			continue;
		}
		
		switch (grid[coordinate])
		{
			case Virus.Weakened:
				++causedInfectionCount;
				grid[coordinate] = Virus.Infected;
				break;
			case Virus.Infected:
				direction = turnRight(direction);
				grid[coordinate] = Virus.Flagged;
				break;
			case Virus.Flagged:
				direction = turnLeft(turnLeft(direction));
				grid[coordinate] = Virus.Clean;
				break;
		}
		
		advance(direction);
	}
}
