<Query Kind="Program" />

void Main()
{
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day2.txt")))
	{
		var rows = GetRows(file);
		
		// Part 1
		// rows.Select(x => x.Max() - x.Min()).Sum().Dump();
		
		// Part 2
		rows.SelectMany(r =>
			r.SelectMany((a, aIndex) =>
				r.Select((b, bIndex) => new { A = a, AIndex = aIndex, B = b, BIndex = bIndex })
			).
			Where(x => x.AIndex != x.BIndex && x.A % x.B == 0).
			Select(x => x.A / x.B)
		).Sum().Dump();
	}
}

// Define other methods and classes here
IEnumerable<IEnumerable<int>> GetRows(StreamReader file)
{
	while (!file.EndOfStream)
	{
		yield return file.ReadLine().Split('\t').Select(Int32.Parse);
	}
}
