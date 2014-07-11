using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace uLearn.Courses.Linq.Slides
{
	[TestFixture]
	public class ReadPointsExcercise
	{
		/*

		##Задача: Чтение из файла 2

		В файле filename в каждой строке написаны две координаты точки, разделенные пробелом.
		Прочитайте файл в массив точек.
		Решите задачу в один LINQ-запрос. Постарайтесь не использовать функцию преобразования строки в число более одного раза.

		###Пример файла
		    1 -2
		    -3 4
		    0 2
		
		Написанная вами функция в дальнейшем может быть использована, например, вот так:
		*/

		[ShowBodyOnSlide]
		public void ParseNumber_Sample()
		{
			List<Point> points = ParsePoints(File.ReadLines("points.txt"));
		}


		[Exercise(SingleStatement = true)]
		[Hint("string.Split — разбивает строку на части по разделителю")]
		[Hint("int.Parse преобразует строку в целое число.")]
		[Hint("Каждую строку нужно преобразовать в точку. Преобразование — это дело для метода Select. ",
			"Но каждая строка — это список координат, каждую из которых нужно преобразовать из строки в число.",
			"Подумайте про Select внутри Select-а.")]
		public List<Point> ParsePoints(IEnumerable<string> lines)
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

		[Test]
		public void Test_ReadPoints()
		{
			List<Point> actualPoints = ParsePoints(new[] {"1 -2", "-3 4"});
			Assert.That(
				actualPoints,
				Is.EqualTo(new[] {new Point(1, -2), new Point(-3, 4)}).AsCollection);
		}

		/*
		### Краткая справка
		  * `IEnumerable<R> Select(this IEnumerable<T> items, Func<T, R> map)`
		  * `List<T> ToList(this IEnumerable<T> items)`
		*/
	}
}