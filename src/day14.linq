<Query Kind="Program" />

void Main()
{
	var input = GetStringInput();
	
	// Part 1
	var grid = GetGrid(input);
	grid.SelectMany(r => r.Select(i => i)).Sum().Dump();
	
	// Part 2
	LabelConsecutiveGroups(grid);
	grid.SelectMany(r => r.Select(i => i)).Where(c => c != 0).Distinct().Count().Dump();
}

// Define other methods and classes here
string GetStringInput()
{
	return "stpzcrnm";
}

public int[][] GetGrid(string input)
{
	var gridStrings = GetGridStrings(input);
	
	var grid = new int[gridStrings.Count][];
	
	for (var i = 0; i < grid.Length; ++i)
	{
		grid[i] = new int[gridStrings[i].Length];
		
		for (var j = 0; j < grid[i].Length; ++j)
		{
			grid[i][j] = gridStrings[i][j] == '1' ? 1 : 0;
		}
	}
	
	return grid;
}

public IList<string> GetGridStrings(string input)
{
	return Enumerable.
		Range(0, 128).
		Select(i => KnotHash(input + "-" + i.ToString())).
		Select(ToByteString).
		ToList();
}

public string ToByteString(string gridRowString)
{
	return gridRowString.
		Select(c => Byte.Parse(c.ToString(), System.Globalization.NumberStyles.HexNumber)).
		Select(i => Convert.ToString(i, 2).PadLeft(4, '0')).
		Aggregate((a, b) => a + b);
}

public void GridReplace(int[][] grid, int originalGroup, int replacementGroup)
{
	for (var i = 0; i < grid.Length; ++i)
		for (var j = 0; j < grid[i].Length; ++j)
			if (grid[i][j] == originalGroup)
				grid[i][j] = replacementGroup;
}

public void SetGroupsToMinusOne(int[][] grid)
{
	for (var i = 0; i < grid.Length; ++i)
		for (var j = 0; j < grid[i].Length; ++j)
			if (grid[i][j] == 1)
				grid[i][j] = -1;
}

public void LabelConsecutiveGroups(int[][] grid)
{
	var regionCounter = 0;
	
	SetGroupsToMinusOne(grid);
	
	for (var i = 0; i < grid.Length; ++i)
	{
		for (var j = 0; j < grid[i].Length; ++j)
		{
			if (grid[i][j] == 0)
				continue;
			
			var region = grid[i][j];
			
			if (j + 1 < grid[i].Length && grid[i][j + 1] != 0)
			{
				if (region > -1 && grid[i][j + 1] > -1)
				{
					var originalGroup = Math.Max(region, grid[i][j + 1]);
					var replacementGroup = Math.Min(region, grid[i][j + 1]);
					
					region = replacementGroup;
					GridReplace(grid, originalGroup, replacementGroup);
				}
				else if (region == -1 && grid[i][j + 1] == -1)
				{
					region = ++regionCounter;
				}
				else if (region == -1 || grid[i][j + 1] == -1)
				{
					region = Math.Max(region, grid[i][j + 1]);
				}

				grid[i][j] = grid[i][j + 1] = region;
			}
			
			if (region == -1)
			{
				region = ++regionCounter;
				grid[i][j] = region;
			}
			
			if (i + 1 < grid.Length && grid[i + 1][j] == -1)
				grid[i + 1][j] = region;
		}
	}
}

// Solution to Day 10
public class CycleList<T> : IEnumerable<T>
{
	public int Offset { get; set; }
	private IList<T> List { get; set; }
	
	public CycleList(IEnumerable<T> sequence)
	{
		List = new List<T>();
		Offset = 0;
		
		foreach (var item in sequence)
			List.Add(item);
	}
	
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
	
	public IEnumerator<T> GetEnumerator()
	{
		for (var i = 0; i < List.Count; ++i)
			yield return this[i];
	}
	
	public CycleList<T> Rotate(int start, int length)
	{
		start %= List.Count;
		
		var rotatedCycleList = new CycleList<T>(
			Sequence(start + length, List.Count - length).
			Concat(
				Sequence(start, length).
				Reverse()
			)
		);
		
		rotatedCycleList.Offset = ((2 * List.Count) - (start + length)) % List.Count;
		
		return rotatedCycleList;
	}
	
	private IEnumerable<T> Sequence(int start, int length)
	{
		for (int i = start; i < start + length; ++i)
			yield return this[i];
	}
	
	public T this[int i]
	{
		get { return List[(i + Offset) % List.Count]; }
		set { List[(i + Offset) % List.Count] = value; }
	}
}

public string KnotHash(string input)
{
	var seed = new CycleList<byte>(Enumerable.Range(0, 256).Select(i => (byte) i));
	var lengths = input.
		Select(c => Convert.ToByte(c)).
		Concat(new byte[] { 17, 31, 73, 47, 23 }).
		ToList();
	
	var skipCounter = 0;
	var position = 0;
	
	foreach (var round in Enumerable.Range(0, 64))
	{
		seed = KnotHash(seed, lengths, ref skipCounter, ref position);
	}
	
	return XorBytes(seed);
}

public string XorBytes(CycleList<byte> bytes)
{
	var xorBytes = bytes.Select((b, i) => new { B = b, I = i }).
		GroupBy(bi => bi.I / 16).
		Select(g => g.
			Select(x => x.B).
			Aggregate((x, y) => (byte) (x ^ y))
		).
		ToArray();
	
	return BitConverter.ToString(xorBytes).ToLower().Replace("-", string.Empty);
}

public CycleList<byte> KnotHash(CycleList<byte> seed, IList<byte> lengths, ref int skipCounter, ref int position)
{
	var skipCounterClosure = skipCounter;
	var positionClosure = position;
	
	Func<CycleList<byte>, int, int, CycleList<byte>> applyRotation = (cycleList, start, length) =>
	{
		var nextCycleList = cycleList.Rotate(start, length);
		positionClosure = (positionClosure + skipCounterClosure++ + length) % cycleList.Count();
		return nextCycleList;
	};
	
	foreach (var length in lengths)
	{
		seed = applyRotation(seed, positionClosure, length);
	}
	
	skipCounter = skipCounterClosure;
	position = positionClosure;
	return seed;
}