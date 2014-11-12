using System;
using System.Collections.Generic;
using System.Linq;
using uLearn.CSharp;

namespace uLearn.Courses.Linq.Slides.LINQ
{
	[Slide("Декартово произведение", "{FF3215D3-5CC7-4C28-83B1-77465F570DC8}")]
	public class S060_CartesianProduct : SlideTestBase
	{
		/*

		Одно из не совсем очевидных применений `SelectMany` — это вычисление декартова произведения двух множеств.
		Опробуйте этот трюк на следующей задаче:

		Вычислить координаты всех восьми соседей заданной точки.

		Можете полагаться на то, что в классе `Point` переопределен метод `Equals`, 
		сравнивающий точки покоординатно.

		*/

		[ExpectedOutput(@"Neighbours of (1 2)
0 1
0 2
0 3
1 1
1 3
2 1
2 2
2 3
Neighbours of (0 0)
-1 -1
-1 0
-1 1
0 -1
0 1
1 -1
1 0
1 1
")]
		[HideOnSlide]
		public static void Main()
		{
			CheckForPoint(new Point(1, 2));
			CheckForPoint(new Point(0, 0));
		}

		[HideOnSlide]
		private static void CheckForPoint(Point point)
		{
			Console.WriteLine("Neighbours of ({0})", point);
			var lines = GetNeighbours(point).OrderBy(p => p.X).ThenBy(p => p.Y).Select(p => p.ToString());
			foreach (var line in lines)
				Console.WriteLine(line);
		}

		[Exercise]
		[Hint("Декартово произведение множества {-1, 0, 1} на себя даст все возможные относительные координаты соседей")]
		[Hint("Используйте вызов Select внутри вызова SelectMany")]
		public static IEnumerable<Point> GetNeighbours(Point p)
		{
			int[] d = { -1, 0, 1 };
			return d
				.SelectMany(dx => d.Select(dy => new Point(p.X + dx, p.Y + dy)))
				.Where(n => !n.Equals(p));
			/*uncomment
			int[] d = {-1, 0, 1}; // используйте подсказку, если не понимаете зачем тут этот массив :)
			return ...
			*/
		}

		[HideOnSlide]
		public static bool IsRightCartesian(IEnumerable<Point> neighbors)
		{
			var correctAnswer = new HashSet<Point>()
			{
				new Point(0, 1),
				new Point(0, 2),
				new Point(0, 3),
				new Point(1, 1),
				new Point(1, 3),
				new Point(2, 1),
				new Point(2, 2),
				new Point(2, 3)
			};
			return neighbors.All(correctAnswer.Contains);
		}

		[HideOnSlide]
		public class Point
		{
			[HideOnSlide]
			public Point(int x, int y)
			{
				X = x;
				Y = y;
			}
			[HideOnSlide]
			public int X, Y;

			[HideOnSlide]
			public override string ToString()
			{
				return string.Format("{0} {1}", X, Y);
			}

			[HideOnSlide]
			protected bool Equals(Point other)
			{
				return X == other.X && Y == other.Y;
			}

			[HideOnSlide]
			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != this.GetType()) return false;
				return Equals((Point)obj);
			}

			[HideOnSlide]
			public override int GetHashCode()
			{
				unchecked
				{
					return (X * 397) ^ Y;
				}
			}
		}

	}
}