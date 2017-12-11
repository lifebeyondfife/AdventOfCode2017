<Query Kind="Program" />

void Main()
{
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day10.txt")))
	{
		var input = GetStringInput(file);
		var lengths = GetLengths(input);
		
		// Part 1
		var seed = new CycleList<byte>(Enumerable.Range(0, 256).Select(i => (byte) i));
		
		var skipCounter = 0;
		var position = 0;
		
		var hashedSeed = KnotHash(seed, lengths, ref skipCounter, ref position);
		(hashedSeed[0] * hashedSeed[1]).Dump();
		
		// Part 2
		var hash = KnotHash(input).Dump();
	}
}

// Define other methods and classes here
string GetStringInput(StreamReader file)
{
	return file.ReadLine();
}

IList<byte> GetLengths(string input)
{
	return input.Split(',').Select(Byte.Parse).ToList();
}

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
