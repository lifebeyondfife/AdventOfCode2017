<Query Kind="Program" />

void Main()
{
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day9.txt")))
	{
		var rawGroupInput = GetRawGroupInput(file);
		
		int removedCharacterCount;
		var groupInput = RemoveGarbage(rawGroupInput, out removedCharacterCount);
		
		// Part 1
		int index = 0;
		var rootGroupNode = new Group();
		ConstructGroups(groupInput, ref index, rootGroupNode).TotalScore.Dump();
		
		// Part 2
		removedCharacterCount.Dump();
	}
}

// Define other methods and classes here
string GetRawGroupInput(StreamReader file)
{
	return file.ReadLine();
}

string RemoveGarbage(string rawGroupInput, out int removedCharacterCount)
{
	var removedCharacterCountClosure = 0;
	var exclamation = false;
	var openAngleBracket = false;
	
	Func<char, bool> takeCharacter =
		character => {
			if (exclamation)
			{
				exclamation = false;
				return false;
			}
			
			if (openAngleBracket && character == '>')
			{
				openAngleBracket = false;
				return false;
			}
			
			if (character == '!')
			{
				exclamation = true;
				return false;
			}
			
			if (character == '<')
			{
				if (openAngleBracket)
					++removedCharacterCountClosure;
				else
					openAngleBracket = true;
				
				return false;
			}
			
			if (openAngleBracket)
			{
				++removedCharacterCountClosure;
				return false;
			}
			
			return true;
		};
	
	var groupInput = new String(rawGroupInput.Where(takeCharacter).ToArray());
	removedCharacterCount = removedCharacterCountClosure;
	
	return groupInput;	
}

public Group ConstructGroups(string groupInput, ref int index, Group parent)
{
	while (index < groupInput.Length && groupInput[index] != '}')
	{
		if (groupInput[index] == ',')
			++index;
		
		if (groupInput[index] == '{')
		{
			++index;
			var @group = new Group(parent);
			parent.Children.Add(ConstructGroups(groupInput, ref index, @group));
		}
	}
	
	++index;
	
	return parent;
}

public class Group
{
	public IList<Group> Children { get; private set; }
	public Group Parent { get; private set; }
	
	public int Score
	{
		get
		{
			return Parent == null ? 0 : Parent.Score + 1;
		}
	}
	
	public int TotalScore
	{
		get
		{
			return Score + Children.Sum(c => c.TotalScore);
		}
	}
	
	public Group()
	{
		Children = new List<Group>();
	}
	
	public Group(Group parent) : this()
	{
		Parent = parent;
	}
}
