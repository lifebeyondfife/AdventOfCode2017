<Query Kind="Program" />

void Main()
{
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day7.txt")))
	{
		var programInputs = GetInput(file).ToList();
		
		var rootNode = FindRootNode(programInputs);
		
		// Part 1
		rootNode.Name.Dump();
		
		// Part 2
		CorrectWeight(rootNode, 0).Dump();
	}
}

// Define other methods and classes here
IEnumerable<Tuple<Program, List<string>>> GetInput(StreamReader file)
{
	while (!file.EndOfStream)
	{
		var input = file.ReadLine();
		
		var split = " -> ";
		var splitPosition = input.IndexOf(split);
		
		var nodeInput = new List<string>();
		var childrenInput = new List<string>();
		
		if (splitPosition < 0)
		{
			nodeInput = input.Split(' ').ToList();
		}
		else
		{
			nodeInput = input.
				Substring(0, splitPosition).
				Split(' ').
				ToList();
			childrenInput = input.
				Substring(splitPosition + split.Length, input.Length - (splitPosition + split.Length)).
				Replace(",", string.Empty).
				Split(' ').
				ToList();
		}
		
		yield return Tuple.Create(
			new Program(nodeInput[0], Int32.Parse(nodeInput[1].Replace("(", string.Empty).Replace(")", string.Empty))),
			childrenInput
		);
	}
}

Program FindRootNode(IList<Tuple<Program, List<string>>> programInputs)
{
	var programLookup = new Dictionary<string, Program>();
	var programStructure = new Dictionary<string, IList<string>>();
	
	foreach (var programInput in programInputs)
	{
		programLookup[programInput.Item1.Name] = programInput.Item1;
		programStructure[programInput.Item1.Name] = programInput.Item2;
	}

	foreach (var programNameList in programStructure)
	{
		foreach (var childName in programNameList.Value)
		{
			programLookup[programNameList.Key].Children.Add(programLookup[childName]);
		}
	}
	
	return programLookup.OrderByDescending(kvp => kvp.Value.Depth).First().Value;
}

int CorrectWeight(Program node, int imbalance)
{
	Func<IList<int>, bool> allEqual = list =>list.Zip(list.Skip(1), (a, b) => a == b).All(x => x);

	var childrenWeights = node.Children.Select(c => c.TotalWeight).ToList();
	
	if (allEqual(childrenWeights))
		return node.Weight + imbalance;
	
	var mean = childrenWeights.Average();
	var majorityWeight = childrenWeights.OrderBy(w => Math.Abs(w - mean)).First();
	var erroneousNode = node.Children.Single(c => c.TotalWeight != majorityWeight);
	
	return CorrectWeight(erroneousNode, majorityWeight - erroneousNode.TotalWeight);
}

public class Program
{
	public string Name { get; private set; }
	public int Weight { get; private set; }
	public IList<Program> Children { get; private set; }
	
	public int Depth
	{
		get
		{
			return Children.Count == 0 ? 0 : Children.Select(c => c.Depth).Max() + 1;
		}
	}
	
	public int TotalWeight
	{
		get
		{
			return Weight + Children.Sum(c => c.TotalWeight);
		}
	}
	
	public Program(string name, int weight)
	{
		Name = name;
		Weight = weight;
		Children = new List<Program>();
	}
}