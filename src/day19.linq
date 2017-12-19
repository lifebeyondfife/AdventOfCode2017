<Query Kind="Program" />

void Main()
{
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day19.txt")))
	{
		var map = GetMap(file);
		var steps = 0;
		
		// Part 1
		var route = Traverse(map, ref steps).Dump();
		
		// Part 2
		steps.Dump();
	}
}

// Define other methods and classes here
public char[][] GetMap(StreamReader file)
{
	var rows = new List<char[]>();
	
	while (!file.EndOfStream)
		rows.Add(file.ReadLine().ToCharArray());
	
	return rows.ToArray();
}

public enum Dir
{
	up,
	down,
	left,
	right
}

public string Traverse(char[][] map, ref int steps)
{
	steps = 0;
	var y = 0;	
	var x = map[0].Select((c, i) => new { C = c, I = i }).Single(ci => ci.C == '|').I;
	
	var route = string.Empty;
	
	var direction = Dir.down;
	
	Func<Dir, char> nextCharacter = dir =>
		{
			switch (dir)
			{
				case Dir.up: --y; break;
				case Dir.down: ++y; break;
				case Dir.left: --x; break;
				case Dir.right: ++x; break;
			}
			
			return map[y][x];
		};
	
	Func<Dir, Dir> nextDirection = dir =>
		{
			switch (dir)
			{
				case Dir.up:
				case Dir.down:
					return map[y][x + 1] == ' ' ? Dir.left : Dir.right;
				
				case Dir.left:
				case Dir.right:
					return map[y + 1][x] == ' ' ? Dir.up : Dir.down;
				
				default:
					throw new Exception("Unknown direction.");
			}
		};
	
	while (true)
	{
		++steps;
		var character = nextCharacter(direction);
		
		if (character == ' ')
			break;
		
		if (character == '|' || character == '-')
			continue;
		
		if (character == '+')
			direction = nextDirection(direction);
		else
			route += character;
	}
	
	return route;
}



