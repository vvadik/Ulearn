using System.Linq;
using NUnit.Framework;

namespace uLearn.Courses.Linq.Slides
{
	[Slide("OrderBy и Distinct", "{CA6CAD1F-8352-4041-9233-DFF70ED3AB05}")]
	[TestFixture]
	public class Sort
	{
		/*

		Для сортировки последовательности в Linq имеется четыре метода:

		    IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> items, Func<T, K> keySelector)
		    IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> items, Func<T, K> keySelector)
		    IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> items, Func<T, K> keySelector)
		    IOrderedEnumerable<T> ThenByDescending<T>(this IOrderedEnumerable<T> items, Func<T, K> keySelector)
		
		Первые две дают на выходе последовательность, упорядоченную по возрастанию/убыванию ключей.
		А `keySelector` — это как раз функция, которая каждому элементу последовательности ставит
		в соответствие некоторый ключ, по которому его и нужно отсортировать.
		*/

		[ShowBodyOnSlide]
		[Test]
		public void Test1()
		{
			var names = new[] {"Pavel", "Alexander", "Anna"};

			IOrderedEnumerable<string> sorted = names.OrderBy(n => n.Length);

			Assert.That(sorted, Is.EqualTo(new[] {"Anna", "Pavel", "Alexander"}));
		}

		/*
		Если при равенстве ключей вы хотите отсотрировать элементы по другому критерию, 
		на помощь приходит метод ThenBy.

		Например, в следующем примере все имена сортируются по убыванию длин, а при равных длинах — лексикографически.
		*/

		[ShowBodyOnSlide]
		[Test]
		public void Test2()
		{
			var names = new[] {"Pavel", "Alexander", "Irina"};
			var sorted = names
				.OrderByDescending(name => name.Length)
				.ThenBy(n => n);
			Assert.That(sorted, Is.EqualTo(new[] {"Alexander", "Irina", "Pavel"}).AsCollection);
		}

		/*
		Чтобы убрать из последовательности все повторяющиеся элементы, можно воспользоваться функцией `Distinct`.
		*/

		[ShowBodyOnSlide]
		[Test]
		public void TestDistinct()
		{
			var numbers = new[] {1, 2, 3, 3, 1, 1,};
			var uniqueNumbers = numbers.Distinct();
			Assert.That(uniqueNumbers.Count(), Is.EqualTo(3));
		}
	}
}