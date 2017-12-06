<Query Kind="Program" />

void Main()
{
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day5.txt")))
	{
		var jumps = GetInput(file).ToList();

		// Part 1
		// FollowJumps(jumps, x => 1).Dump();
		
		// Part 2
		FollowJumps(jumps, x => x >= 3 ? -1 : 1).Dump();
	}
}

// Define other methods and classes here
IEnumerable<int> GetInput(StreamReader file)
{
	while (!file.EndOfStream)
	{
		yield return Int32.Parse(file.ReadLine());
	}
}

int FollowJumps(IList<int> jumps, Func<int, int> modifierRule)
{
	var jumpCounter = 0;
	var index = 0;
	
	do
	{
		var previousIndex = index;
		
		index += jumps[index];
		jumps[previousIndex] += modifierRule(jumps[previousIndex]);
		
		++jumpCounter;	
	} while (index >= 0 && index < jumps.Count);

	return jumpCounter;
}