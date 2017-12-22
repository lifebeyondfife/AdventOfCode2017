<Query Kind="Program" />

void Main()
{
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day22.txt")))
	{
		var grid = new Grid<bool>(GetGridStream(file));
		//var grid = new Grid<bool>(new [] { false, false, true, true, false, false, false, false, false });
		
		//5213 is too low
		//5214 is too low
		
		var burstIterations = 10000;
		int causedInfectionCount;
		
		Simulate(grid, burstIterations, out causedInfectionCount);
		
		causedInfectionCount.Dump();
	}
}

// Define other methods and classes here
IEnumerable<bool> GetGridStream(StreamReader file)
{
	foreach (var boolean in Enumerable.Repeat(false, 27))
		yield return boolean;
	while (!file.EndOfStream)
	{
		yield return false;
		
		foreach (var character in file.ReadLine())
			yield return character == '#' ? true : false;
		
		yield return false;
	}
	foreach (var boolean in Enumerable.Repeat(false, 27))
		yield return boolean;
}

public class Grid<T> where T : struct
{
	public List<T> Buffer { get; set; }
	private int Offset { get; set; }
	private int EdgeLength { get; set; }
	
	public Grid(IEnumerable<T> buffer)
	{
		Buffer = buffer.ToList();
		EdgeLength = (int) Math.Sqrt(Buffer.Count);
		Offset = EdgeLength / 2;
	}
	
	public Grid(int size) :
		this(new List<T>(Enumerable.Repeat(new T(), size)))
	{
	}
	
	private void CheckBounds(int i, int j)
	{
		if ((i + Offset) * EdgeLength + (j + Offset) < 0 ||
			(i + Offset) * EdgeLength + (j + Offset) >= Buffer.Count)
			DoubleBuffer();
	}
	
	private void DoubleBuffer()
	{
	//	Console.WriteLine("doubling the buffer");
		var newGrid = new Grid<T>(4 * EdgeLength * EdgeLength);
		
		for (var i = -Offset; i <= Offset; ++i)
		{
			for (var j = -Offset; j <= Offset; ++j)
			{
				if ((i + Offset) * EdgeLength + (j + Offset) < 0 ||
					(i + Offset) * EdgeLength + (j + Offset) >= Buffer.Count)
						continue;
			
				newGrid[i, j] = this[i, j];
			}
		}
		
		Buffer = newGrid.Buffer;
		Offset = newGrid.Offset;
		EdgeLength = newGrid.EdgeLength;
	}
	
	public void PrintBuffer()
	{
		var @default = new T();
		
		foreach (var group in Buffer.
			Select((x, i) => new { X = x, I = i }).
			GroupBy(xi => xi.I / EdgeLength))
				Console.WriteLine(group.
					Select(xi => !xi.X.Equals(@default) ? "#" : ".").
					Aggregate((x, y) => x + y));
		
//		for (var i = -Offset; i <= Offset; ++i)
//		{
//			if (i == Offset && Buffer.Count % 2 == 0)
//				continue;
//			
//			for (var j = -Offset; j <= Offset; ++j)
//			{
//				if (j == Offset && Buffer.Count % 2 == 0)
//					continue;
//				
//				if (x == i && y == j)
//					if (!this[i, j].Equals(@default))
//						Console.Write("X");
//					else
//						Console.Write("x");
//				else
//					Console.Write(!this[i, j].Equals(@default) ? "#" : ".");
//			}
//			Console.WriteLine();
//		}
//		Console.WriteLine();
	}
	
	public T this[int i, int j]
	{
		get
		{
			CheckBounds(i, j);
			return Buffer[(i + Offset) * EdgeLength + (j + Offset)];
		}
		
		set
		{
			CheckBounds(i, j);
			Buffer[(i + Offset) * EdgeLength + (j + Offset)] = value;
		}
	}
}

public enum Direction
{
	up,
	down,
	left,
	right
}

public void Simulate(Grid<bool> grid, int burstIterations, out int causedInfectionCount)
{
	causedInfectionCount = 0;
	var x = 0;
	var y = 0;
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
			case Direction.up: --y; break;
			case Direction.left: --x; break;
			case Direction.down: ++y; break;
			case Direction.right: ++x; break;
		}
	};
	
	while (--burstIterations >= 0)
	{
		direction = grid[y, x] ? turnRight(direction) : turnLeft(direction);
		
		if (!grid[y, x])
		{
			++causedInfectionCount;
			grid[y, x] = true;
		}
		else
			grid[y, x] = false;
		
		advance(direction);
	}
}


