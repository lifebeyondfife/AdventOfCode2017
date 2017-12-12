<Query Kind="Program" />

void Main()
{
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day12.txt")))
	{
		var graph = GetGraph(file);
		
		ISet<int> discoveredEdges = new HashSet<int>();
		DepthFirstSearch(graph, 0, ref discoveredEdges);
		
		// Part 1
		discoveredEdges.Count.Dump();
		
		// Part 2
		GroupCount(graph).Dump();
	}
}

// Define other methods and classes here
IDictionary<int, IList<int>> GetGraph(StreamReader file)
{
	var regexTemplate = @"([0-9]*) <-> ([0-9, ]*)";
	var graph = new Dictionary<int, IList<int>>();
	
	while (!file.EndOfStream)
	{
		var match = Regex.Match(file.ReadLine(), regexTemplate);
		var key = Int32.Parse(match.Groups[1].Value);
		var value = match.Groups[2].Value.Replace(" ", string.Empty).Split(',').Select(Int32.Parse).ToList();
		
		graph[key] = value;
	}
	
	return graph;
}

public void DepthFirstSearch(IDictionary<int, IList<int>> graph, int node, ref ISet<int> discoveredEdges)
{
	discoveredEdges.Add(node);
	
	foreach (var neighbour in graph[node])
	{
		if (!discoveredEdges.Contains(neighbour))
			DepthFirstSearch(graph, neighbour, ref discoveredEdges);
	}
}

public int GroupCount(IDictionary<int, IList<int>> graph)
{
	var groups = 0;
	ISet<int> discoveredEdges = new HashSet<int>();
	
	while (graph.Keys.Any())
	{
		DepthFirstSearch(graph, graph.Keys.First(), ref discoveredEdges);

		foreach (var node in discoveredEdges)
			graph.Remove(node);
			
		discoveredEdges.Clear();
		
		++groups;
	}
	
	return groups;
}
