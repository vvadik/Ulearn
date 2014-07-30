using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace uLearn.Courses.Linq.Slides
{
	[Slide("Take, Skip, ToArray, ToList", "{11c65f91-8698-48d9-9a97-ba5b42d27133}")]
	public class AdditionalMethods
	{
		/*
		 Пора познакомиться ещё с несколькими простыми, но часто используемыми методами.

		`IEnumerable<T> Take(this IEnumerable<T> items, int count)`

		`Take` обрезает последовательность, после указанного количества элементов.
		
		`IEnumerable<T> Skip(this IEnumerable<T> items, int count)`

		`Skip` обрезает последовательность, пропуская указанное количество элементов сначала.

		`ToArray` и `ToList` используются для преобразования `IEnumerable<T>` в массив `T[]` или в `List<T>`, соответственно.
		
		Ещё раз отметим, что все эти методы не меняют исходную коллекцию, а возвращают новую последовательность.

		А вот и пример, показывающий работу всех этих методов вместе:
		*/

		[Test]
		[ShowBodyOnSlide]
		public void BasicFunctional()
		{
			int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

			IEnumerable<int> even = numbers.Where(x => x % 2 == 0);
			IEnumerable<int> squares = even.Select(x => x * x);
			IEnumerable<int> squaresWithoutOne = squares.Skip(1);
			IEnumerable<int> secondAndThirdSquares = squaresWithoutOne.Take(2);
			int[] result = secondAndThirdSquares.ToArray();

			Assert.That(result, Is.EqualTo(new[] { 16, 36 }));
		}

	}
}