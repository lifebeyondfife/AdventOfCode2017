<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day18.txt")))
	{
		var instructions = GetInstructions(file).ToList();

		// Part 1
		var recoveredSound = ExecuteSerial(instructions, GetRegisters(instructions)).Dump();
		
		// Part 2
		SentInstructions(instructions).Dump();
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
			case Command.add:	add(instruction.Item2[0], instruction.Item2[1]); break;
			case Command.jgz:	jgz(instruction.Item2[0], instruction.Item2[1]); break;
			case Command.mod:	mod(instruction.Item2[0], instruction.Item2[1]); break;
			case Command.mul:	mul(instruction.Item2[0], instruction.Item2[1]); break;
			case Command.set:	set(instruction.Item2[0], instruction.Item2[1]); break;
			case Command.snd:	snd(instruction.Item2[0]); break;
			
			case Command.rcv:
				if (rcv(instruction.Item2[0]))
					return lastPlayedSound;
				break;
		}
	}
}

public int SentInstructions(IList<Tuple<Command, List<string>>> instructions)
{
	var registers0 = GetRegisters(instructions);
	var registers1 = GetRegisters(instructions);
	
	registers0["p"] = 0;
	registers1["p"] = 1;
	
	var queue0 = new Queue<long>();
	var queue1 = new Queue<long>();
	
	var sent0 = 0;
	var sent1 = 0;
	
	var nextInstruction0 = 0;
	var nextInstruction1 = 0;
	
	do
	{
		ExecuteParallel(instructions, registers0, queue0, queue1, ref sent0, ref nextInstruction0);
		ExecuteParallel(instructions, registers1, queue1, queue0, ref sent1, ref nextInstruction1);
	} while (queue0.Any() || queue1.Any());
	
	return sent1;
}

public void ExecuteParallel(
	IList<Tuple<Command, List<string>>> instructions,
	IDictionary<string, long> registers,
	Queue<long> sendQueue,
	Queue<long> receiveQueue,
	ref int sent,
	ref int nextInstruction)
{
	Func<string, long> val = v => Value(registers, v);
	
	Action<string> snd = x => sendQueue.Enqueue(val(x));
	Func<string, bool> rcv = x =>
		{
			if (!receiveQueue.Any())
				return false;
			
			registers[x] = receiveQueue.Dequeue();
			return true;
		};
	Action<string, string> set = (x, y) => registers[x] = val(y);
	Action<string, string> add = (x, y) => registers[x] += val(y);
	Action<string, string> mul = (x, y) => registers[x] *= val(y);
	Action<string, string> mod = (x, y) => registers[x] %= val(y);
	Func<string, string, int> jgz = (x, y) =>
		{
			if (val(x) > 0)
				return (int) val(y) - 1;
			return 0;
		};
	
	while (true)
	{
		var instruction = instructions[nextInstruction++];
		
		switch (instruction.Item1)
		{
			case Command.add:	add(instruction.Item2[0], instruction.Item2[1]); break;
			case Command.jgz:	nextInstruction += jgz(instruction.Item2[0], instruction.Item2[1]); break;
			case Command.mod:	mod(instruction.Item2[0], instruction.Item2[1]); break;
			case Command.mul:	mul(instruction.Item2[0], instruction.Item2[1]); break;
			case Command.set:	set(instruction.Item2[0], instruction.Item2[1]); break;
			case Command.snd:	snd(instruction.Item2[0]); ++sent; break;
			case Command.rcv:   if (!rcv(instruction.Item2[0])) { --nextInstruction; return; } break;
		}
	}
}