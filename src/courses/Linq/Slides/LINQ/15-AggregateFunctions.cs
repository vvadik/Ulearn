using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace uLearn.Courses.Linq.Slides
{
	[Slide("Функции аггрегирования", "{A6BACA4C-D211-428B-AE49-1F6882FD67F1}")]
	[TestFixture]
	public class AggregateFunctions
	{
		/*
		В `Linq` есть удобные методы для вычисления минимума, максимума, среднего и количества элементов в последовательности.
		
		Вот все они в действии:
		*/

		[Test]
		[ShowBodyOnSlide]
		public void MinMaxAvg()
		{
			IEnumerable<int> nums = new int[] {8, 9, 0, 1, 2, 3, 4, 5, 6, 7};
			string[] words = {"hello", "kittie"};

			Assert.That(nums.Count(), Is.EqualTo(10));

			Assert.That(nums.Min(), Is.EqualTo(0));

			Assert.That(words.Select(word => word.Length).Max(), Is.EqualTo(6));
			// Можно записать строку выше короче, если воспользоваться другой перегрузкой агрегатной функции.
			// Подобные перегрузки есть у всех агрегатных функций
			Assert.That(words.Max(word => word.Length), Is.EqualTo(6));

			Assert.That(nums.Average(n => n*n), Is.EqualTo(28.5));
		}

		/*
		Все эти методы при вызове полностью обходят коллекцию.
		Исключение составляет только метод `Count` — если последовательность на самом деле реализует интерфейс ICollection
		(в котором есть свойство `Count`), то `Linq`-метод `Count()` не станет перебирать всю коллекцию, а сразу вернет значение свойства `Count`.

		Благодаря этой оптимизации, временная сложность работы `Linq`-метода `Count()` 
		на массивах, списках, хэш-таблицах и многих других структурах данных — `O(1)`.
		*/

		/*
		Есть ещё две полезные функции: `All` и `Any`, которые проверяют, выполняется ли заданный предикат для всех элементов
		последовательности или хотя бы для одного элемента соответственно.
		*/

		[Test]
		[ShowBodyOnSlide]
		public void Test2()
		{
			int[] numbers = {1, 2, 6, 2, 8, 0, 10, 6, 1, 2};

			Assert.That(numbers.All(n => n >= 0), Is.EqualTo(true));
			Assert.That(numbers.All(n => n%2 == 0), Is.EqualTo(false));

			Assert.That(numbers.Any(n => n == 0), Is.EqualTo(true));
			Assert.That(numbers.Any(n => n < 0), Is.EqualTo(false));
		}
	}
}