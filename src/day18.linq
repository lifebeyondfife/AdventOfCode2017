<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.Tasks.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.Tasks.Parallel.dll</Reference>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day18.txt")))
	{
		var instructions = GetInstructions(file).ToList();
//		var instructions = new Tuple<Command, List<string>> [] 
//			{
//				Tuple.Create(Command.snd, new [] {"1"}.ToList()),
//				Tuple.Create(Command.snd, new [] {"2"}.ToList()),
//				Tuple.Create(Command.snd, new [] {"p"}.ToList()),
//				Tuple.Create(Command.rcv, new [] {"a"}.ToList()),
//				Tuple.Create(Command.rcv, new [] {"b"}.ToList()),
//				Tuple.Create(Command.rcv, new [] {"c"}.ToList()),
//				Tuple.Create(Command.rcv, new [] {"d"}.ToList())
//			};

		// 13715 is too high
		
		// Part 1
		//var recoveredSound = ExecuteSerial(instructions, GetRegisters(instructions)).Dump();
		
		State.Initialise();
		
		var tokenSource = new CancellationTokenSource();
		var token = tokenSource.Token;

		var programA = Task.Factory.StartNew(() => ExecuteParallel(instructions, GetRegisters(instructions), 0), token);
		var programB = Task.Factory.StartNew(() => ExecuteParallel(instructions, GetRegisters(instructions), 1), token);
		
		while (true)
		{
			if (State.IsEmptyA && State.IsEmptyB)
			{
				State.IsEmptyA.Dump();
				State.IsEmptyB.Dump();
				Thread.Sleep(5000);
				
				if (State.IsEmptyA && State.IsEmptyB)
					break;
			}
		}
		
		tokenSource.Cancel();
		
		// Part 2
		State.UpdateCountProgramB.Dump();
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
	snd,
	set,
	add,
	mul,
	mod,
	rcv,
	jgz
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

public long ExecuteSerial(IList<Tuple<Command, List<string>>> instructions, IDictionary<string, long> registers)
{
	var lastPlayedSound = 0L;
	var nextInstruction = 0;
	
	Func<string, long> val = v => Value(registers, v);
	
	Action<string> snd = s => 
		{
			if (val(s) > 0)
				lastPlayedSound = val(s);
		};
	Action<string, string> set = (x, y) => registers[x] = val(y);
	Action<string, string> add = (x, y) => registers[x] += val(y);
	Action<string, string> mul = (x, y) => registers[x] *= val(y);
	Action<string, string> mod = (x, y) => registers[x] %= val(y);
	Func<string, bool> rcv = x => val(x) != 0;
	Action<string, string> jgz = (x, y) =>
		{
			if (val(x) > 0)
				nextInstruction = nextInstruction + (int) val(y) - 1;
		};
	
	while (true)
	{
		var instruction = instructions[nextInstruction++];
		
		switch (instruction.Item1)
		{
			case Command.add:	add(instruction.Item2[0], instruction.Item2[1]); val(instruction.Item2[0]); break;
			case Command.jgz:	jgz(instruction.Item2[0], instruction.Item2[1]); val(instruction.Item2[0]); break;
			case Command.mod:	mod(instruction.Item2[0], instruction.Item2[1]); val(instruction.Item2[0]); break;
			case Command.mul:	mul(instruction.Item2[0], instruction.Item2[1]); val(instruction.Item2[0]); break;
			case Command.set:	set(instruction.Item2[0], instruction.Item2[1]); val(instruction.Item2[0]); break;
			case Command.snd:	snd(instruction.Item2[0]); val(instruction.Item2[0]); break;
			
			case Command.rcv:
				if (rcv(instruction.Item2[0]))
					return lastPlayedSound;
				break;
		}
	}
}

public static class State
{
	private static Queue<long> ProgramA { get; set; }
	private static Queue<long> ProgramB { get; set; }
	private static Object LockA = new Object();
	private static Object LockB = new Object();
	public static bool IsEmptyA { get; private set; }
	public static bool IsEmptyB { get; private set; }

	public static int UpdateCountProgramB { get; private set; }
	
	public static void Initialise()
	{
		ProgramA = new Queue<long>();
		ProgramB = new Queue<long>();
		IsEmptyA = false;
		IsEmptyB = false;
		
		UpdateCountProgramB = 0;
	}
	
	public static long Receive(int channel)
	{
		channel.Dump("Receive");
		ProgramA.Dump("ProgramA");
		ProgramB.Dump("ProgramB");
		if (channel == 0)
		{
			while (ProgramA.Count == 0)
				IsEmptyA = true;
			
			lock (LockA)
			{
				IsEmptyA = false;
				return ProgramA.Dequeue();
			}
		}
		else
		{
			while (ProgramB.Count == 0)
				IsEmptyB = true;
			
			lock (LockB)
			{
				IsEmptyB = false;
				return ProgramB.Dequeue();
			}
		}
	}
	
	public static void Send(int channel, long value)
	{
		channel.Dump("Send");
		ProgramA.Dump("ProgramA");
		ProgramB.Dump("ProgramB");
		if (channel == 0)
		{
			lock (LockA)
			{
				ProgramA.Enqueue(value);
			}
		}
		else
		{
			lock (LockB)
			{
				++UpdateCountProgramB;
				UpdateCountProgramB.Dump();
				ProgramB.Enqueue(value);
			}
		}
	}
}

public long ExecuteParallel(IList<Tuple<Command, List<string>>> instructions, IDictionary<string, long> registers, int channel)
{
	registers["p"] = channel;
	var nextInstruction = 0;
	Func<string, long> val = v => Value(registers, v);
	
	Action<string> snd = x => State.Send(channel, val(x));
	Action<string> rcv = x => registers[x] = State.Receive(channel);
	Action<string, string> set = (x, y) => registers[x] = val(y);
	Action<string, string> add = (x, y) => registers[x] += val(y);
	Action<string, string> mul = (x, y) => registers[x] *= val(y);
	Action<string, string> mod = (x, y) => registers[x] %= val(y);
	Action<string, string> jgz = (x, y) =>
		{
			if (val(x) > 0)
				nextInstruction = nextInstruction + (int) val(y) - 1;
		};
	
	while (true)
	{
		var instruction = instructions[nextInstruction++];
		
		switch (instruction.Item1)
		{
			case Command.add:	add(instruction.Item2[0], instruction.Item2[1]); val(instruction.Item2[0]); break;
			case Command.jgz:	jgz(instruction.Item2[0], instruction.Item2[1]); val(instruction.Item2[0]); break;
			case Command.mod:	mod(instruction.Item2[0], instruction.Item2[1]); val(instruction.Item2[0]); break;
			case Command.mul:	mul(instruction.Item2[0], instruction.Item2[1]); val(instruction.Item2[0]); break;
			case Command.set:	set(instruction.Item2[0], instruction.Item2[1]); val(instruction.Item2[0]); break;
			case Command.snd:	snd(instruction.Item2[0]); break;
			case Command.rcv:   rcv(instruction.Item2[0]); break;
		}
	}
}