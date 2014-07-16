using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NUnit.Framework;

namespace uLearn.Courses.Linq.Slides
{
	[Title("Задача №4")]
	[TestFixture]
	public class CartesianProduct
	{
		/*

		##Задача: Соседи
		
		Одно из не совсем очевидных применений `SelectMany` — это вычисление декартова произвдеения двух множеств.
		Попробуйте применить это знание при решении следующей задачи:

		Дана точка. Нужно вернуть `HashSet<Point>`, содержащий всех соседей этой точки в смысле 8-связности.

		*/

		[Exercise(SingleStatement = false)]
		[Hint("Декартово произведение множества {-1, 0, 1} на себя даст все возможные относительные координаты соседей")]
		[Hint("Используйте вызов Select внутри вызова SelectMany")]
		[Hint("Метода ToHashSet не существует, однако у класса HashSet<T> есть конструктор, принимающие последовательность")]
		public static HashSet<Point> GetNeighbours(Point p)
		{
			int[] d = {-1, 0, 1};
			return new HashSet<Point>(
				d
					.SelectMany(dx => d.Select(dy => new Point(p.X + dx, p.Y + dy)))
					.Where(n => !n.Equals(p))
				);
			/*uncomment
			int[] d = {-1, 0, 1};
			return ...
			*/
		}

		[ExpectedOutput("Good")]
		[ShowOnSlide]
		public static void Main()
		{
			var answer = GetNeighbours(new Point(1, 2));
			var result = IsRightCartesian(answer) ? "Good" : "Bad";
			Console.WriteLine(result);
		}

		public static bool IsRightCartesian(IEnumerable<Point> neighbours)
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
			return neighbours.All(correctAnswer.Contains);
		}
	}
}