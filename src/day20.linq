<Query Kind="Program" />

void Main()
{
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day20.txt")))
	{
		IList<Particle> particles = GetParticles(file).ToList();

		var origin = new Vector(0L, 0L, 0L);
		
		// Part 1
		particles.
			Select(p => new { p.Index, Position = p.Position(1000) }).
			OrderBy(p => p.Position.ManhattanDistance(origin)).
			First().
			Index.
			Dump();
		
		// Part 2
		Simulate(ref particles);
		
		particles.Count.Dump();
	}
}

// Define other methods and classes here
public IEnumerable<Particle> GetParticles(StreamReader file)
{
	var regexTemplate = @"p=<([-0-9]*),([-0-9]*),([-0-9]*)>, v=<([-0-9]*),([-0-9]*),([-0-9]*)>, a=<([-0-9]*),([-0-9]*),([-0-9]*)>";
	var index = 0;
	
	while (!file.EndOfStream)
	{
		var match = Regex.Match(file.ReadLine(), regexTemplate);
		
		yield return new Particle(
			index++,
			new Vector(Int64.Parse(match.Groups[1].Value), Int64.Parse(match.Groups[2].Value), Int64.Parse(match.Groups[3].Value)),
			new Vector(Int64.Parse(match.Groups[4].Value), Int64.Parse(match.Groups[5].Value), Int64.Parse(match.Groups[6].Value)),
			new Vector(Int64.Parse(match.Groups[7].Value), Int64.Parse(match.Groups[8].Value), Int64.Parse(match.Groups[9].Value))
		);
	}
}

public struct Vector
{
	public long X;
	public long Y;
	public long Z;
	
	public Vector(long x, long y, long z)
	{
		X = x;
		Y = y;
		Z = z;
	}
	
	public long ManhattanDistance(Vector other)
	{
		return Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
	}
	
	public static Vector operator+(Vector a, Vector b)
	{
		return new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	}
}

public class Particle
{
	public Vector Start { get; private set; }
	public int Index { get; private set; }
	public Vector Current { get; private set; }
	
	private Vector Velocity { get; set; }
	private Vector Acceleration { get; set; }
	
	public Particle(int index, Vector start, Vector velocity, Vector acceleration)
	{
		Index = index;
		Start = start;
		Velocity = velocity;
		Acceleration = acceleration;
		Current = Start;
	}
	
	public Vector Position(long time)
	{
		Func<long, long, long, long> distance = (u, a, t) => (u * t) + (a * t * t) / 2;
		
		return new Vector(
			Start.X + distance(Velocity.X, Acceleration.X, time),
			Start.Y + distance(Velocity.Y, Acceleration.Y, time),
			Start.Z + distance(Velocity.Z, Acceleration.Z, time)
		);
	}
	
	public void Move()
	{
		Velocity += Acceleration;
		Current += Velocity;
	}
}

public void Simulate(ref IList<Particle> particles)
{
	foreach (var step in Enumerable.Range(0, 100))
	{
		var collisions = new Dictionary<Vector, IList<Particle>>();
		foreach (var particle in particles)
		{
			particle.Move();
			
			if (collisions.ContainsKey(particle.Current))
				collisions[particle.Current].Add(particle);
			else
				collisions[particle.Current] = new List<Particle>(new [] { particle });
		}
		
		foreach (var collided in collisions.Select(kvp => kvp.Value).Where(l => l.Count > 1).SelectMany(x => x))
			particles.Remove(collided);
	}
}
