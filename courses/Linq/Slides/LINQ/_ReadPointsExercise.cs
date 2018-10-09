using System;
using System.Collections.Generic;
using System.Linq;
using uLearn.CSharp;

namespace uLearn.Courses.Linq.Slides.LINQ.LINQ.LINQ
{
	public class S030_ReadPointsExercise : SlideTestBase
	{
		/*

		Теперь у вас есть список строк, в каждой из которой написаны две координаты точки (X, Y), разделенные пробелом.

		Реализуйте метод `ParsePoints` в одно `LINQ`-выражение.
		*/

		public static void Main()
		{
			// Функция тестирования ParsePoints

			foreach (var point in ParsePoints(new[] { "1 -2", "-3 4", "0 2" }))
				Console.WriteLine(point.X + " " + point.Y);
			foreach (var point in ParsePoints(new List<string> { "+01 -0042" }))
				Console.WriteLine(point.X + " " + point.Y);
		}

		public class Point
		{
			public Point(int x, int y)
			{
				X = x;
				Y = y;
			}
			public int X, Y;
		}

		/*
		Постарайтесь не использовать функцию преобразования строки в число более одного раза.
		*/

		public static List<Point> ParsePoints(IEnumerable<string> lines)
		{
			return lines
				.Select(line => line.Split(' ').Select(int.Parse).ToList())
				.Select(nums => new Point(nums[0], nums[1]))
				.ToList();
			/*uncomment
			return lines
				.Select(...)
				...
			*/
		}
	}
}