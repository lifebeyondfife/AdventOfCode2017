<Query Kind="Program" />

void Main()
{
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day16.txt")))
	{
		var instructions = GetInstructions(file).ToList();
		
		var alphabet = "abcdefghijklmnop";
		
		var programs = new CycleList<string>(alphabet.Select(x => x.ToString()));
		
		var cycleLength = 0;
		while (true)
		{
			foreach (var instruction in instructions)
				instruction.Apply(programs);
			
			++cycleLength;
			
			// Part 1
			if (cycleLength == 1)
				programs.Aggregate((x, y) => x + y).Dump();
			
			if (alphabet == programs.Aggregate((x, y) => x + y))
				break;
		}
		
		foreach (var range in Enumerable.Range(0, 1000000000 % cycleLength))
			foreach (var instruction in instructions)
				instruction.Apply(programs);
		
		// Part 2
		programs.Aggregate((x, y) => x + y).Dump();
	}
}

// Define other methods and classes here
IEnumerable<Instruction<string>> GetInstructions(StreamReader file)
{
	var instructions = file.ReadLine().Split(',');
	
	foreach (var instruction in instructions)
	{
		var operation = (Operation) Enum.Parse(typeof(Operation), instruction.Substring(0, 1));
		var operand = instruction.Substring(1);
		
		Func<Operation, string, IExecute<string>> execute = (op, oper) =>
		{
			if (op == Operation.s)
				return new Spin<string>(Int32.Parse(oper));
			
			var operands = oper.Split('/');
			
			if (op == Operation.x)
				return new Exchange<string>(Int32.Parse(operands[0]), Int32.Parse(operands[1]));
			
			return new Partner<string>(operands[0], operands[1]);
		};
		
		yield return new Instruction<string>(execute(operation, operand));
	}
}

public class CycleList<T> : IEnumerable<T>
{
	public int Offset { get; set; }
	private IList<T> List { get; set; }
	private IDictionary<T, int> Lookup { get; set; }
	
	public CycleList(IEnumerable<T> sequence)
	{
		List = new List<T>();
		Offset = 0;
		
		foreach (var item in sequence)
			List.Add(item);
		
		Lookup = List.
			Select((x, i) => new { X = x, I = i }).
			ToDictionary(x => x.X, x => x.I);		
	}
	
	public void Spin(int spin)
	{
		Offset = (Offset + (List.Count - spin)) % List.Count;
	}
	
	public void Exchange(int firstPosition, int secondPosition)
	{
		var firstValue = this[firstPosition];
		var secondValue = this[secondPosition];

		this[firstPosition] = secondValue;
		this[secondPosition] = firstValue;
		
		Lookup[firstValue] = ((secondPosition + List.Count) + (Offset - List.Count)) % List.Count;
		Lookup[secondValue] = ((firstPosition + List.Count) + (Offset - List.Count)) % List.Count;
	}
	
	public void Partner(T firstValue, T secondValue)
	{
		Exchange((List.Count + Lookup[firstValue] - Offset) % List.Count,
			(List.Count + Lookup[secondValue] - Offset) % List.Count);
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
	
	public T this[int i]
	{
		get { return List[(i + Offset) % List.Count]; }
		set { List[(i + Offset) % List.Count] = value; }
	}
}

public enum Operation
{
	p,
	s,
	x
}

public class Instruction<T>
{
	public IExecute<T> Execute { get; private set; }
	
	public Instruction(IExecute<T> execute)
	{
		Execute = execute;
	}
	
	public void Apply(CycleList<T> cycleList)
	{
		Execute.Apply(cycleList);
	}
}

public interface IExecute<T>
{
	void Apply(CycleList<T> cycleList);	
}

public class Spin<T> : IExecute<T>
{
	private int Steps { get; set; }
	
	public Spin(int steps)
	{
		Steps = steps;
	}
	
	void IExecute<T>.Apply(CycleList<T> cycleList)
	{
		cycleList.Spin(Steps);
	}
}

public class Exchange<T> : IExecute<T>
{
	private int FirstPosition { get; set; }
	private int SecondPosition { get; set; }
	
	public Exchange(int firstPosition, int secondPosition)
	{
		FirstPosition = firstPosition;
		SecondPosition = secondPosition;
	}
	
	void IExecute<T>.Apply(CycleList<T> cycleList)
	{
		cycleList.Exchange(FirstPosition, SecondPosition);
	}
}

public class Partner<T> : IExecute<T>
{
	private T FirstValue { get; set; }
	private T SecondValue { get; set; }
	
	public Partner(T firstValue, T secondValue)
	{
		FirstValue = firstValue;
		SecondValue = secondValue;
	}
	
	void IExecute<T>.Apply(CycleList<T> cycleList)
	{
		cycleList.Partner(FirstValue, SecondValue);
	}
}
