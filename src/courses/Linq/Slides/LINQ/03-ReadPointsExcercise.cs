using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace uLearn.Courses.Linq.Slides
{
	[Title("Задача. Чтение списка точек")]
	[TestFixture]
	public class ReadPointsExcercise
	{
		/*

		В файле filename в каждой строке написаны две координаты точки, разделенные пробелом.
		Прочитайте файл в массив точек.
		Решите задачу в один LINQ-запрос. Постарайтесь не использовать функцию преобразования строки в число более одного раза.

		###Пример файла
		    1 -2
		    -3 4
		    0 2
		
		Написанная вами функция в дальнейшем может быть использована, например, вот так:
		*/
		[ExpectedOutput("0,0\r\n1,1\r\n2,2")]
		[ShowOnSlide]
		public static void Main()
		{
			List<Point> points = ParsePoints(new string[]{"0 0","1 1","2 2"});
			foreach (var e in points)
				Console.WriteLine("{0},{1}", e.X, e.Y);
		}

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
				.Select(nums => new Point(nums[0], nums[1]))
				.ToList();
			/*uncomment
			return lines
				.Select(...)
				...
			*/
		}
		/*
		### Краткая справка
		  * `IEnumerable<R> Select(this IEnumerable<T> items, Func<T, R> map)`
		  * `List<T> ToList(this IEnumerable<T> items)`
		*/
	}
}