<Query Kind="Program" />

void Main()
{
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day6.txt")))
	{
		var bankIntegers = GetInput(file);
		var bank = new MemoryBank(bankIntegers);
		var bankHistory = new SortedSet<MemoryBank>();
		
		// Part 1
		while (!bankHistory.Contains(bank))
		{
			bankHistory.Add(bank);
			bank = bank.Redistribute();
		}

		bankHistory.Count.Dump();

		// Part 2
		var infiniteLoopBankHistory = new SortedSet<MemoryBank>();
		while (!infiniteLoopBankHistory.Contains(bank))
		{
			infiniteLoopBankHistory.Add(bank);
			bank = bank.Redistribute();
		}		
		
		infiniteLoopBankHistory.Count.Dump();
	}
}

// Define other methods and classes here
IList<int> GetInput(StreamReader file)
{
	return file.ReadLine().Split('\t').Select(Int32.Parse).ToList();
}

public class MemoryBank : IComparable
{
	public IList<int> Banks { get; private set; }
	
	public MemoryBank(IList<int> banks)
	{
		Banks = banks;
	}
	
	public MemoryBank Redistribute()
	{
		var rebalanceIndex = Banks.
			Select((b, i) => new { Bank = b, Index = i }).
			OrderByDescending(x => x.Bank).
			Select(x => x.Index).
			First();
		
		return new MemoryBank(new List<int>(Banks)).Distribute(rebalanceIndex);		
	}
	
	private MemoryBank Distribute(int index)
	{
		var blocks = Banks[index];
		Banks[index] = 0;
		
		while (blocks > 0)
		{
			index = (index + 1) % Banks.Count;
			
			++Banks[index];
			--blocks;
		}
		
		return this;
	}
	
	public int CompareTo(object obj)
	{
		if (obj == null)
			return 1;
		
		var otherMemoryBank = obj as MemoryBank;
		if (otherMemoryBank == null || otherMemoryBank.Banks.Count != Banks.Count)
			throw new ArgumentException("Object is not valid");
		
		var compare = Banks.Zip(otherMemoryBank.Banks, (t, o) => new { This = t, Other = o }).
			SkipWhile(x => x.This == x.Other);
		
		if (!compare.Any())
			return 0;
		else
			return compare.Select(x => x.This.CompareTo(x.Other)).First();
	}
}
