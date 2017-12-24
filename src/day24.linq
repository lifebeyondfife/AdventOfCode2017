<Query Kind="Program" />

void Main()
{
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day24.txt")))
	{
		var connectors = GetConnectors(file).ToList();
		
		Func<List<Tuple<int, int>>, bool> highScore = list =>
			list.Sum(c => c.Item1 + c.Item2) > State.HighScore;
		
		var chain = new List<Tuple<int, int>>();
		OptimalChain(new List<Tuple<int, int>>(connectors), 0, ref chain, highScore);
		
		// Part 1
		State.HighScore.Dump();
		
		Func<List<Tuple<int, int>>, bool> longestChain = list => 
			list.Count > State.Longest ||
			(list.Count == State.Longest && list.Sum(c => c.Item1 + c.Item2) > State.HighScore);
		
		State.HighScore = State.Longest = 0;
		
		chain = new List<Tuple<int, int>>();
		OptimalChain(new List<Tuple<int, int>>(connectors), 0, ref chain, longestChain);
		
		// Part 2
		State.HighScore.Dump();
	}
}

// Define other methods and classes here
IEnumerable<Tuple<int, int>> GetConnectors(StreamReader file)
{
	while (!file.EndOfStream)
	{
		var connectorPins = file.ReadLine().Split('/');
		yield return Tuple.Create(Int32.Parse(connectorPins.First()), Int32.Parse(connectorPins.Last()));
	}
}

public static class State
{
	public static int HighScore = 0;
	public static int Longest = 0;
}

public void OptimalChain(IList<Tuple<int, int>> connectors, int pinRequired, ref List<Tuple<int, int>> chain, Func<List<Tuple<int, int>>, bool> chainMetric)
{
	foreach (var connector in connectors.Where(c => c.Item1 == pinRequired || c.Item2 == pinRequired))
	{
		chain.Add(connector);
		
		OptimalChain(new List<Tuple<int, int>>(connectors.Where(c => c != connector)),
			connector.Item1 == pinRequired ? connector.Item2 : connector.Item1,
			ref chain,
			chainMetric);
		
		chain.RemoveAt(chain.Count - 1);
	}
	
	if (chainMetric(chain))
	{
		State.HighScore = chain.Sum(c => c.Item1 + c.Item2);
		State.Longest = chain.Count;
	}
}

