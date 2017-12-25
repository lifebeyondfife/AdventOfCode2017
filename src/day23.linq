<Query Kind="Program" />

void Main()
{
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day23.txt")))
	{
		var instructions = GetInstructions(file).ToList();

		// Part 1
		var multiplesCount = Execute(instructions, GetRegisters(instructions)).Dump();

		// Part 2
		var nonPrimes = ReverseEngineeredAssemblyCode(57).Dump();
	}
}

// Define other methods and classes here
IEnumerable<Tuple<Command, List<string>>> GetInstructions(StreamReader file)
{
	while (!file.EndOfStream)
	{
		var parts = file.ReadLine().Split(' ');
		
		yield return Tuple.Create((Command) (Enum.Parse(typeof(Command), parts[0])), parts.Skip(1).ToList());
	}
}

public enum Command
{
	set,
	sub,
	mul,
	jnz	
}

public IDictionary<string, long> GetRegisters(IEnumerable<Tuple<Command, List<string>>> instructions)
{
	return instructions.
		SelectMany(i => i.Item2).
		Where(s => !s.Contains("-") && !s.All(Char.IsDigit)).
		Distinct().
		ToDictionary(s => s, v => 0L);
}

public long Value(IDictionary<string, long> registers, string reference)
{
	long value;
	return Int64.TryParse(reference, out value) ? value : registers[reference];
}

public long Execute(IList<Tuple<Command, List<string>>> instructions, IDictionary<string, long> registers)
{
	var nextInstruction = 0;
	
	Func<string, long> val = v => Value(registers, v);
	
	Action<string, string> set = (x, y) => registers[x] = val(y);
	Action<string, string> sub = (x, y) => registers[x] -= val(y);
	Action<string, string> mul = (x, y) => registers[x] *= val(y);
	Action<string, string> jnz = (x, y) =>
		{
			if (val(x) != 0)
				nextInstruction = nextInstruction + (int) val(y) - 1;
		};
	
	var multiplierCount = 0;
	while (instructions.Count > nextInstruction)
	{
		var instruction = instructions[nextInstruction++];
		
		switch (instruction.Item1)
		{
			case Command.set:	set(instruction.Item2[0], instruction.Item2[1]); break;
			case Command.sub:	sub(instruction.Item2[0], instruction.Item2[1]); break;
			case Command.jnz:	jnz(instruction.Item2[0], instruction.Item2[1]); break;
			case Command.mul:	mul(instruction.Item2[0], instruction.Item2[1]); ++multiplierCount; break;
		}

	}
	
	return multiplierCount;
}

public int ReverseEngineeredAssemblyCode(int seed)
{
	var lowerBound = (seed * 100) + 100000;
	var upperBound = lowerBound + 17000;
	
	var primes = Enumerable.Range(lowerBound, upperBound - lowerBound + 1).
		ToDictionary(x => x, x => true);
	
	foreach (var factor in Enumerable.Range(2, (int) Math.Sqrt(upperBound)))
	{
		var innerFactor = 1;
		while (innerFactor * factor < upperBound)
			primes[++innerFactor * factor] = false;
	}
	
	return Enumerable.Range(0, 1001).
		Select(x => lowerBound + 17 * x).
		Where(x => !primes[x]).
		Count();
}

public int AssemblyCode(bool isDebugMode)
{
	var a = isDebugMode ? 0 : 1;
	var b = 0;
	var c = 0;
	var d = 0;
	var e = 0;
	var f = 0;
	var g = 0;
	var h = 0;
	
	b = 57;
	c = b;
	
	if (a != 0)
	{
		b = (b * 100) + 100000;
		c = b + 17000;
	}
	
	do
	{
		f = 1;
		d = 2;
		do
		{
			e = 2;
			do
			{
				g = d * e - b;
				if (g == 0)
					f = 0;
				g = ++e - b;
			} while (g != 0);
			g = ++d - b;
		} while (g != 0);
		if (f == 0)
			++h;
		g = b - c;
		if (g == 0)
			break;
		b = b + 17;
	} while (true);

	return h;
}
