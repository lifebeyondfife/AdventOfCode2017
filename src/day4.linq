<Query Kind="Program" />

void Main()
{
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day4.txt")))
	{
		var rows = GetRows(file);
		
		// Part 1
		// rows.Count(IsValidPassphrase).Dump();
		
		// Part 2
		rows.Count(IsValidPassphraseNoAnagrams).Dump();
	}
}

// Define other methods and classes here
IEnumerable<IList<string>> GetRows(StreamReader file)
{
	while (!file.EndOfStream)
	{
		yield return file.ReadLine().Split(' ');
	}
}

bool IsValidPassphrase(IList<string> passphrase)
{
	var occurrences = new HashSet<string>();
	
	foreach (var word in passphrase)
	{
		if (occurrences.Contains(word))
			return false;
		else
			occurrences.Add(word);
	}

	return true;
}

bool IsValidPassphraseNoAnagrams(IList<string> passphrase)
{
	var occurrences = new HashSet<string>();
	
	foreach (var word in passphrase)
	{
		var orderedWord = new String(word.OrderBy(x => x).ToArray());
		
		if (occurrences.Contains(orderedWord))
			return false;
		else
			occurrences.Add(orderedWord);
	}

	return true;
}
