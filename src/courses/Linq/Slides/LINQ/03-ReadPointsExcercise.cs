using System;
using System.Collections.Generic;
using System.Linq;

namespace uLearn.Courses.Linq.Slides
{
	[Slide("Задача. Чтение списка точек", "{563307C9-F265-4EA0-B06E-8390582F718E}")]
	public class ReadPointsExcercise
	{
		/*

		В файле в каждой строке написаны две координаты точки (X, Y), разделенные пробелом.
		Прочитайте файл в массив точек.

		Решите задачу в один LINQ-запрос. Постарайтесь не использовать функцию преобразования строки в число более одного раза.

		###Пример файла
		    1 -2
		    -3 4
		    0 2
		
		Считайте, что за вас уже вызвали функцию `File.ReadLines(filename)` и вашей функции пердают строки, прочитанные из файла:
		*/
		[ExpectedOutput("1 -2\n-3 4\n0 2\n1 -42")]
		[ShowOnSlide]
		public static void Main()
		{
			foreach (var point in ParsePoints(new[] { "1 -2", "-3 4", "0 2" }))
				Console.WriteLine(point.X + " " + point.Y);
			foreach (var point in ParsePoints(new List<string> { "+01 -0042" }))
				Console.WriteLine(point.X + " " + point.Y);
		}
		[ShowOnSlide]
		public class Point { public int X, Y; }

		[Exercise(SingleStatement = true)]
		[Hint("string.Split — разбивает строку на части по разделителю")]
		[Hint("int.Parse преобразует строку в целое число.")]
		[Hint("Каждую строку нужно преобразовать в точку. Преобразование — это дело для метода Select. ",
			"Но каждая строка — это список координат, каждую из которых нужно преобразовать из строки в число.",
			"Подумайте про Select внутри Select-а.")]
		public static List<Point> ParsePoints(IEnumerable<string> lines)
		{
			return lines
				.Select(line => line.Split(' ').Select(int.Parse).ToList())
				.Select(nums => new Point {X = nums[0], Y = nums[1]})
				.ToList();
			/*uncomment
			return lines
				.Select(...)
				...
			*/
		}
	}
}