namespace linqed
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Drawing;
	using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;
	using NUnit.Framework;

	public class Point
	{
		public Point(int x, int y)
		{
			X = x;
			Y = y;
		}
		public int X, Y;
	}
	public class ZipLinqed_Test
	{
		// Zip
		public IEnumerable<Point> MakePoints(int[] xs, int[] ys)
		{
			return xs.Zip(ys, (x, y) => new Point(x, y));
		}

		// Cartesian product
		public IEnumerable<Tuple<string, string, int>> GetMostSimilarWordsPairs(string[] words)
		{
			return words.SelectMany(
				w1 => words.Select(w2 =>
					Tuple.Create(
						w1,
						w2,
						w1.Zip(w2, (c1, c2) => c1 == c2 ? 0 : 1).Sum() + Math.Abs(w1.Length - w2.Length))
					))
				.OrderBy(t => t.Item3);
		}
	}

	public class SerializationLinqed
	{
		// string.Join
		public string Serialize(IEnumerable<double[]> points)
		{
			Func<double[], string> toString = p => string.Join("  ", p.Select(x => x.ToString()));
			return string.Join("\r\n", points.Select(toString));
		}

		// Concat
		public void SaveToJsonFile(string filename, IEnumerable<Point> points)
		{
			File.WriteAllLines(filename,
				new[] {"["}
					.Concat(points.Select(p => string.Format("{{x={0}, y={1}}}", p.X, p.Y)))
					.Concat(new[] {"]"}));
		}
	}

	public class ShuffleLinqed
	{
		//Enumerable.Range
		public T[] Shuffle1<T>(T[] items, Random random)
		{
			return Enumerable.Range(0, items.Length)
				.OrderBy(i => random.Next())
				.Select(i => items[i]).ToArray();
		}

		public T[] Shuffle2<T>(T[] items, Random random)
		{
			return
				items
					.Select(item => Tuple.Create(random.Next(), item))
					.OrderBy(t => t.Item1)
					.Select(t => t.Item2)
					.ToArray();
		}

		// yield return
		public IEnumerable<T> Shuffle3<T>(T[] items, Random random)
		{
			for (int i = 0; i < items.Length; i++)
			{
				var next = random.Next(items.Length - i);
				yield return items[next];
				items[next] = items[items.Length - 1 - i];
			}
		}
	}


	public class SmoothingLinqed
	{
		// Closure 
		public static IEnumerable<double> ExpSmooth(IEnumerable<double> data, double e)
		{
			if (e < 0 || e >= 1) throw new ArgumentException();
			double sum = 0;
			return data.Select(v => sum = sum*e + (1 - e)*v);
		}

		// yield return. Lazy data processing.
		public static IEnumerable<double> MovingAverage(IEnumerable<double> data, int windowSize)
		{
			var window = new Queue<double>();
			var sum = 0.0;
			foreach (var x in data)
			{
				if (window.Count > windowSize) sum -= window.Dequeue();
				window.Enqueue(x);
				sum += x;
				yield return sum/window.Count;
			}
		}
	}

	public class GameOfLifeLinqed
	{
		public IEnumerable<Point> GetNextGeneration(IEnumerable<Point> aliveCells)
		{
			Func<Point, IEnumerable<Point>> getNeighbours =
				p =>
					from dx in Enumerable.Range(-1, 3)
					from dy in Enumerable.Range(-1, 3)
					where dx != 0 || dy != 0
					select new Point(p.X + dx, p.Y + dy);

			var aliveSet = new HashSet<Point>(aliveCells);
			return
				from p in aliveSet
				from n in getNeighbours(p)
				let count = getNeighbours(n).Count(aliveSet.Contains)
				where count == 3 || count == 2 && aliveSet.Contains(n)
				select n;
		}
	}

	[TestFixture]
	public class Anagrams_Test
	{
		[Test]
		[Explicit]
		public void Test()
		{
			var sw = Stopwatch.StartNew();
			var anagrams = File.ReadLines("d:\\wordlist.txt")
				.Select(word => new {word, hash = new string(word.OrderBy(c => c).ToArray())})
				.GroupBy(w => w.hash)
				.Select(g => string.Join(" ", g.Select(w => w.word)));
			File.WriteAllLines("d:\\anagrams.txt", anagrams);
			Console.WriteLine(sw.Elapsed);
		}
	}

	public static class ExtendingLinq
	{
		public static IEnumerable<T> Except<T>(this IEnumerable<T> items, Func<T, bool> predicate)
		{
			return items.Where(item => !predicate(item));
		}
	}
}