<Query Kind="Program" />

void Main()
{
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day25.txt")))
	{
		int diagnosticChecksum;
		PopulateStates(file, out diagnosticChecksum);
		
		foreach (var step in Enumerable.Range(0, diagnosticChecksum))
			State.Next.Execute();
		
		State.Tape.Where(c => c == 1).Sum().Dump();
	}
}

// Define other methods and classes here
void PopulateStates(StreamReader file, out int diagnosticChecksum)
{
	var startState = new String(file.ReadLine().Reverse().Skip(1).Take(1).ToArray());
	diagnosticChecksum = Int32.Parse(file.ReadLine().Split(' ')[5]);
	file.ReadLine();
	
	while (!file.EndOfStream)
	{
		var stateName = new String(file.ReadLine().Reverse().Skip(1).Take(1).ToArray());
		file.ReadLine();
		
		var zeroConditionWrite = Int32.Parse(new String(file.ReadLine().Reverse().Skip(1).Take(1).ToArray()));
		var zeroConditionStep = file.ReadLine().Split(' ').Last() == "right." ? 1 : -1;
		var zeroConditionNextState = new String(file.ReadLine().Reverse().Skip(1).Take(1).ToArray());
		file.ReadLine();
		
		var oneConditionWrite = Int32.Parse(new String(file.ReadLine().Reverse().Skip(1).Take(1).ToArray()));
		var oneConditionStep = file.ReadLine().Split(' ').Last() == "right." ? 1 : -1;
		var oneConditionNextState = new String(file.ReadLine().Reverse().Skip(1).Take(1).ToArray());
		file.ReadLine();
		
		State.States[stateName] = new State(stateName, zeroConditionWrite, zeroConditionStep, zeroConditionNextState,
			oneConditionWrite, oneConditionStep, oneConditionNextState);
	}
	
	State.Next = State.States[startState];
}


public class State
{
	public static IList<int> Tape { get; private set; }
	public static int TapeIndex { get; private set; }
	public static State Next { get; set; }
	public static IDictionary<string, State> States { get; private set; }
	
	public string Name { get; set; }
	
	private int ZeroConditionWrite { get; set; }
	private int ZeroConditionStep { get; set; }
	private string ZeroConditionNextState { get; set; }
	
	private int OneConditionWrite { get; set; }
	private int OneConditionStep { get; set; }
	private string OneConditionNextState { get; set; }
	
	static State()
	{
		Tape = new List<int>(new [] { 0 });
		TapeIndex = 0;
		States = new Dictionary<string, State>();
	}
	
	public State(string name, int zeroConditionWrite, int zeroConditionStep, string zeroConditionNextState,
		int oneConditionWrite, int oneConditionStep, string oneConditionNextState)
	{
		Name = name;
		ZeroConditionWrite = zeroConditionWrite;
		ZeroConditionStep = zeroConditionStep;
		ZeroConditionNextState = zeroConditionNextState;
		OneConditionWrite = oneConditionWrite;
		OneConditionStep = oneConditionStep;
		OneConditionNextState = oneConditionNextState;
	}
	
	public void Execute()
	{
		if (Tape[TapeIndex] == 0)
		{
			Tape[TapeIndex] = ZeroConditionWrite;
			TapeIndex += ZeroConditionStep;
			Next = State.States[ZeroConditionNextState];
		}
		else
		{
			Tape[TapeIndex] = OneConditionWrite;
			TapeIndex += OneConditionStep;
			Next = State.States[OneConditionNextState];
		}
		
		if (TapeIndex < 0)
		{
			Tape.Insert(0, 0);
			++TapeIndex;
		}
		else if (TapeIndex == Tape.Count)
		{
			Tape.Insert(Tape.Count, 0);
		}
	}
}