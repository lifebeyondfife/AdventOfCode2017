<Query Kind="Program" />

void Main()
{
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day13.txt")))
	{
		var layers = GetLayers(file);
		
		var severity = default(int);
		TraverseFirewall(layers, 0, ref severity, false);
		
		// Part 1
		severity.Dump();
		
		// Part 2
		var delay = DelayCount(layers).Dump();
	}
}

// Define other methods and classes here
IDictionary<int, int> GetLayers(StreamReader file)
{
	var layers = new Dictionary<int, int>();
	while (!file.EndOfStream)
	{
		var kvp = file.ReadLine().Replace(" ", string.Empty).Split(':').Select(Int32.Parse);
		
		layers[kvp.First()] = kvp.Last();
	}
	
	return layers;
}

public bool TraverseFirewall(IDictionary<int, int> layers, int delay, ref int severity, bool severityZero)
{
	bool caught = false;
	severity = 0;
	
	foreach (var layerKvp in layers)
	{
		if (((layerKvp.Key + delay) % ((layerKvp.Value - 1) * 2)) == 0)
		{
			if (severityZero)
				return true;
			
			caught = true;
			severity += layerKvp.Key * layerKvp.Value;
		}
	}
	
	return caught;
}

public int DelayCount(IDictionary<int, int> layers)
{
	var delay = 0;
	var severity = 0;
	
	while (TraverseFirewall(layers, delay, ref severity, true))
		++delay;
	
	return delay;
}
