<Query Kind="Program" />

void Main()
{
	var seedA = 679L;
	var seedB = 771L;
	
	var factorA = 16807L;
	var factorB = 48271L;
	
	var divisor = 2147483647L;
	
	State.PreviousA = seedA;
	State.PreviousB = seedB;
	
	Func<long> nextA = () =>
	{
		State.PreviousA = (State.PreviousA * factorA) % divisor;
		return State.PreviousA;
	};
	
	Func<long> nextB = () =>
	{
		State.PreviousB = (State.PreviousB * factorB) % divisor;
		return State.PreviousB;
	};
	
	Func<long, long, bool> lastTwoBytesMatch = (x, y) => (x & 0xFFFF) == (y & 0xFFFF);

	// Part 1
	Generator(nextA, null).
		Zip(Generator(nextB, null), lastTwoBytesMatch).
		Take(40000000).
		Where(x => x).
		Count().
		Dump();

	// Part 2
	State.PreviousA = 679L;
	State.PreviousB = 771L;
	
	Generator(nextA, 4).
		Zip(Generator(nextB, 8), lastTwoBytesMatch).
		Take(5000000).
		Where(x => x).
		Count().
		Dump();
}

// Define other methods and classes here
public static class State
{
	public static long PreviousA { get; set; }
	public static long PreviousB { get; set; }
}

public IEnumerable<long> Generator(Func<long> next, int? multiple)
{
	while (true)
		if (!multiple.HasValue)
			yield return next();
		else
		{
			var value = next();
			if (value % multiple == 0)
				yield return value;
		}
}
