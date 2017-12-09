<Query Kind="Program" />

void Main()
{
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day8.txt")))
	{
		var instructions = GetInput(file).ToList();
		var registers = CreateRegisters(instructions);
		
		int maxRegisterValue;
		ProcessInstructions(registers, instructions, out maxRegisterValue);

		// Part 1
		registers.Max(kvp => kvp.Value).Dump();
		
		// Part 2
		maxRegisterValue.Dump();
	}
}

// Define other methods and classes here
IEnumerable<Instruction> GetInput(StreamReader file)
{
	var regexTemplate = @"([a-z]*) (inc|dec) ([-0-9]*) if ([a-z]*) ([\<\>\=\!]*) ([-0-9]*)";
	
	while (!file.EndOfStream)
	{
		var match = Regex.Match(file.ReadLine(), regexTemplate);
		
		yield return new Instruction(
			match.Groups[1].Value,
			(Operator) Enum.Parse(typeof(Operator), match.Groups[2].Value),
			Int32.Parse(match.Groups[3].Value),
			match.Groups[4].Value,
			StringToComparator(match.Groups[5].Value),
			Int32.Parse(match.Groups[6].Value)
		);
	}
}

public IDictionary<string, int> CreateRegisters(IEnumerable<Instruction> instructions)
{
	var registers = new Dictionary<string, int>();
	
	foreach (var instruction in instructions)
		registers[instruction.Operand] = 0;
	
	return registers;
}

public void ProcessInstructions(IDictionary<string, int> registers, IEnumerable<Instruction> instructions, out int maxRegisterValue)
{
	maxRegisterValue = 0;
	
	foreach (var instruction in instructions)
	{
		if (instruction.Condition(registers))
			instruction.Modification(registers);
		
		if (registers[instruction.Operand] > maxRegisterValue)
			maxRegisterValue = registers[instruction.Operand];
	}
}

public enum Operator
{
	inc,
	dec
}

public static int Operate(int registerValue, Operator oper, int modifier)
{
	switch (oper)
	{
		case Operator.inc:	return registerValue + modifier;
		case Operator.dec:	return registerValue - modifier;
	}
	
	throw new ApplicationException("Unknown operator.");
}

public enum Comparator
{
	EqualTo,
	GreaterThanOrEqualTo,
	LessThanOrEqualTo,
	GreaterThan,
	LessThan,
	NotEqualTo
}

public static Comparator StringToComparator(string comparator)
{
	switch (comparator)
	{
		case "==":
			return Comparator.EqualTo;
		case ">=":
			return Comparator.GreaterThanOrEqualTo;
		case "<=":
			return Comparator.LessThanOrEqualTo;
		case ">":
			return Comparator.GreaterThan;
		case "<":
			return Comparator.LessThan;
		case "!=":
			return Comparator.NotEqualTo;
	}
	
	throw new ApplicationException("Unknown comparator string.");
}

public static bool Compare(int registerValue, Comparator comparator, int condition)
{
	switch (comparator)
	{
		case Comparator.EqualTo:
			return registerValue == condition;
		case Comparator.GreaterThanOrEqualTo:
			return registerValue >= condition;
		case Comparator.LessThanOrEqualTo:
			return registerValue <= condition;
		case Comparator.GreaterThan:
			return registerValue > condition;
		case Comparator.LessThan:
			return registerValue < condition;
		case Comparator.NotEqualTo:
			return registerValue != condition;
	}
	
	throw new ArgumentException("Unknown operator.");
}

public class Instruction
{
	public Func<IDictionary<string, int>, bool> Condition { get; private set; }
	public Action<IDictionary<string, int>> Modification { get; private set; }
	
	public string Operand { get; set; }
		
	public Instruction(string operand1, Operator oper, int modifier, string operand2, Comparator comparator, int condition)
	{
		Operand = operand1;
		Condition = registers => Compare(registers[operand2], comparator, condition);
		Modification = registers => registers[operand1] = Operate(registers[operand1], oper, modifier);
	}
}
