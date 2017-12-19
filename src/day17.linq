<Query Kind="Program" />

void Main()
{
	var jump = 337;
	
	var imageMap = CalculateImageMap(jump, 2017);
	
	// Part 1
	imageMap.SkipWhile(x => x != 2017).Skip(1).First().Dump();
	
	// Part 2
	AfterZero(jump, 50000000).Dump();
}

// Define other methods and classes here
public class CircularLinkedList<T> : IEnumerable<T>
{
	private LinkedList<T> List { get; set; }
	public LinkedListNode<T> Current { get; set; }
	
	public CircularLinkedList(IEnumerable<T> elements)
	{
		List = new LinkedList<T>();
		
		foreach (var element in elements)
			List.AddLast(element);
		
		Current = List.First;
	}
	
	public void AddAfter(LinkedListNode<T> node, T element)
	{
		Current = List.AddAfter(node, element);
	}
	
	public IEnumerable<LinkedListNode<T>> Nodes()
	{
		var node = Current;
		
		while (true)
		{
			yield return node;
			
			if (node == List.Last)
				node = List.First;
			else
				node = node.Next;
		}
	}
	
	public IEnumerator<T> GetEnumerator()
	{
		return List.GetEnumerator();
	}
	
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}

public CircularLinkedList<int> CalculateImageMap(int jump, int iterations)
{
	var imageMap = new CircularLinkedList<int>(new [] { 0 });
	
	foreach (var element in Enumerable.Range(1, iterations))
	{
		var next = imageMap.Nodes().Skip(jump % element).First();
		
		imageMap.AddAfter(next, element);
	}
	
	return imageMap;
}

public int AfterZero(int jump, int iterations)
{
	var afterZero = 0;
	var index = 0;
	
	foreach (var element in Enumerable.Range(1, iterations))
	{
		index = (((index + jump) % element) + 1) % element;
		
		if (index == 0)
			afterZero = element;
	}
	
	return afterZero;
}
