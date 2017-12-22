<Query Kind="Program">
  <Reference Relative="Group.Net.dll">C:\Users\IainMcDonald\Desktop\AdventOfCode2017\src\Group.Net.dll</Reference>
  <Namespace>Group.Net</Namespace>
  <Namespace>Group.Net.Groups</Namespace>
</Query>

void Main()
{
	// CRITICAL - You need the .dll from Group.Net
	// https://github.com/lifebeyondfife/Group.Net
	
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day21.txt")))
	{
		var rules = GetMappingRules(file).ToList();
		
		IList<Image<byte>> images = new [] { new Image<byte>(new byte[] { 0, 1, 0, 0, 0, 1, 1, 1, 1 }.ToList()) }.ToList();

		Iterate(ref images, rules, 5);

		// Part 1
		LongImage(images).Count(x => x == 1).Dump();

		images = new [] { new Image<byte>(new byte[] { 0, 1, 0, 0, 0, 1, 1, 1, 1 }.ToList()) }.ToList();

		Iterate(ref images, rules, 18);

		// Part 1
		LongImage(images).Count(x => x == 1).Dump();

	}
}

// Define other methods and classes here
IEnumerable<Rule> GetMappingRules(StreamReader file)
{
	var separator = " => ";
	while (!file.EndOfStream)
	{
		var rule = file.ReadLine();
		
		var condition = rule.Substring(0, rule.IndexOf(separator));
		var map = rule.Substring(rule.IndexOf(separator) + separator.Length);
		
		yield return new Rule(
			condition.Where(c => c != '/').Select(c => c == '.' ? (byte) 0 : (byte) 1).ToList(),
			map.Where(c => c != '/').Select(c => c == '.' ? (byte) 0 : (byte) 1).ToList()
		);
	}
}

public static class Groups
{
	public static PermutationGroup D4_on_4 = new PermutationGroup(new [] { new Element(new [] { 2, 0, 3, 1 }), new Element(new [] { 0, 2, 1, 3 })});
	public static PermutationGroup D4_on_9 = new PermutationGroup(new [] { new Element(new [] { 6, 3, 0, 7, 4, 1, 8, 5, 2 }), new Element(new [] { 0, 3, 6, 1, 4, 7, 2, 5, 8 })});
}

public class Rule
{
	public Image<byte> Condition { get; private set; }
	public Image<byte> Map { get; private set; }
	private PermutationGroup Group { get; set; }
	
	public Rule(IList<byte> condition, IList<byte> map)
	{
		Condition = new Image<byte>(condition);
		Map = new Image<byte>(map);
		Group = Condition.Count % 2 == 0 ? Groups.D4_on_4 : Groups.D4_on_9;
	}
	
	public bool Match(Image<byte> image)
	{
		if (image.Count != Condition.Count || image.Count(x => x != 0) != Condition.Count(x => x != 0))
			return false;
		
		foreach (var permuation in Group.Orbit(new [] { image }))
		{
			if (permuation.Zip(Condition, (a, b) => a == b).All(x => x))
				return true;
		}
		
		return false;
	}
}

public static class Extensions
{
	public static int Size<T>(this IList<T> list) where T : IComparable<T>
	{
		return list.Count % 2 == 0 ? 2 : 3;
	}

	public static IList<T> Row<T>(this Image<T> image, int row) where T : IComparable<T>
	{
		var sqrt = (int) Math.Sqrt(image.Count);
		return image.
			Skip(row * sqrt).
			Take(sqrt).
			ToList();
	}
}

public Image<byte> ApplyRules(Image<byte> image, IList<Rule> rules)
{
	foreach (var rule in rules)
	{
		if (!rule.Match(image))
			continue;
		
		return rule.Map;
	}
	
	return null;
}

public IList<byte> LongImage(IList<Image<byte>> images)
{
	var longImage = new List<byte>();

	var outerBlockSize = (int) Math.Sqrt(images.Count);
	var innerBlockSize = (int) Math.Sqrt(images[0].Count);
	
	foreach (var blockRow in Enumerable.Range(0, outerBlockSize))
	{
		var rows = images.
			Skip(outerBlockSize * blockRow).
			Take(outerBlockSize).
			ToList();
		
		foreach (var b in Enumerable.
			Range(0, innerBlockSize).
			SelectMany(r => rows.
				Select(im => im.Row(r))
			).
			SelectMany(x => x))
				longImage.Add(b);
	}

	return longImage;
}

public IList<Image<byte>> Decompose(IList<Image<byte>> images)
{
	var longImage = LongImage(images);
	
	var imageWidth = longImage.Size();
	var imageSize = imageWidth * imageWidth;
	var imageCount = longImage.Count / imageSize;
	var imageIndexSize = (int) Math.Sqrt(imageCount);
	var longImageWidth = (int) Math.Sqrt(longImage.Count);
	
	var decomposedImages = new List<Image<byte>>();
	
	foreach (var image in Enumerable.Range(0, imageCount))
	{
		var row = image / imageIndexSize;
		var column = image % imageIndexSize;
		
		decomposedImages.Add(new Image<byte>(Enumerable.Range(0, imageWidth).
			SelectMany(i => Enumerable.Range(0, imageWidth).
				Select(j => longImage[(row * imageWidth * longImageWidth) + (column * imageWidth) + i * longImageWidth + j])
			).ToList()));
	}
	
	return decomposedImages;
}

public void Iterate(ref IList<Image<byte>> images, IList<Rule> rules, int iterations)
{
	foreach (var step in Enumerable.Range(0, iterations))
	{
		images = images.Select(i => ApplyRules(i, rules)).ToList();
		images = Decompose(images);
	}
}