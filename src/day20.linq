<Query Kind="Program" />

void Main()
{
	using (var file = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ".\\day20.txt")))
	{
		var particles = GetParticles(file).ToList();
		
		/*var particles = new []
		{
			new Particle(0, new Vector(-6, 0, 0), new Vector(3, 0, 0), new Vector(0, 0, 0)),
			new Particle(1, new Vector(-4, 0, 0), new Vector(2, 0, 0), new Vector(0, 0, 0)),
			new Particle(2, new Vector(-2, 0, 0), new Vector(1, 0, 0), new Vector(0, 0, 0)),
			new Particle(3, new Vector(3, 0, 0), new Vector(-1, 0, 0), new Vector(0, 0, 0)),
		}.ToList();*/
		
		particles.Count.Dump();
		var origin = new Vector(0L, 0L, 0L);
		
		// Part 1
//		particles.
//			Select(p => new { p.Index, Position = p.Position(100000) }).
//			OrderBy(p => p.Position.ManhattanDistance(origin)).
//			First().
//			Index.
//			Dump();
		
		// Part 2
		
		var p195 = particles.Single(p => p.Index == 195).Dump();
		var p196 = particles.Single(p => p.Index == 196).Dump();
		
		foreach (var step in Enumerable.Range(0, 9))
		{
			p195.Position(step).Dump("P195 at " + step);
			p196.Position(step).Dump("P196 at " + step);

			var collisions = new Dictionary<Vector, IList<Particle>>();
			
			foreach (var particle in particles)
			{
				var position = particle.Position(step);
				
				if (collisions.ContainsKey(position))
				{
					collisions[position].Add(particle);
					collisions[position].Dump("Duplicate at time step " + step);
				}
				else
					collisions[position] = new List<Particle>(new [] { particle });
			}
						
			foreach (var collided in collisions.Select(kvp => kvp.Value).Where(l => l.Count > 1).SelectMany(x => x))
				particles.Remove(collided.Dump(string.Format("Collision at time {0}", step)));
		}
		
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
}

public class Particle
{
	public Vector Start { get; private set; }
	public int Index { get; private set; }
	public Vector Velocity { get; set; }
	public Vector Acceleration { get; set; }
	
	public Particle(int index, Vector start, Vector velocity, Vector acceleration)
	{
		Index = index;
		Start = start;
		Velocity = velocity;
		Acceleration = acceleration;
	}
	
	public Vector Position(long time)
	{
		Func<long, long, long, long> distance = (u, a, t) => u + a * t;
		
		return new Vector(
			Start.X + distance(Velocity.X, Acceleration.X, time),
			Start.Y + distance(Velocity.Y, Acceleration.Y, time),
			Start.Z + distance(Velocity.Z, Acceleration.Z, time)
		);
	}
}