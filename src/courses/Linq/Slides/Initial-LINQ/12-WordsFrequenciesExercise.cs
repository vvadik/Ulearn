using System;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace uLearn.Courses.Linq.Slides
{
	[TestFixture]
	public class GroupingExercise
	{
		/*

		##Задача: Частотный словарь текста

		Дан текст, нужно вывести count наиболее часто встречающихся в тексте слов, вместе с их частотой.

		Напомним сигнатуры всех Linq-методов, которые вам понадобятся в этом упражнении:

		    IEnumerable<IGrouping<K, T>> GroupBy(this IEnumerable<T> items, Func<T, K> keySelector)
			IEnumerable<R> Select(this IEnumerable<T> items, Func<T, R> map)
		    IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> items, Func<T, K> keySelector)
		    IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> items, Func<T, K> keySelector)
		    IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> items, Func<T, K> keySelector)
		    IOrderedEnumerable<T> ThenByDescending<T>(this IOrderedEnumerable<T> items, Func<T, K> keySelector)
		    IEnumerable<T> Take(this IEnumerable<T> items, int count)

		*/

		[Exercise(SingleStatement = true)]
		public Tuple<string, int>[] GetMostFrequentWords(string text, int count)
		{
			return Regex.Split(text, @"\W+")
				.Where(word => word != "")
				.GroupBy(w => w.ToLower())
				.Select(group => Tuple.Create(group.Key, group.Count()))
				.OrderByDescending(tuple => tuple.Item2)
				.ThenBy(tuple => tuple.Item1)
				.Take(count)
				.ToArray();
			/*uncomment
			return Regex.Split(text, @"\W+")
				.Where(word => word != "")
				// ваш код
				.ToArray();
			*/
		}

		[Test]
		public void Test()
		{
			var words = GetMostFrequentWords("You?! Or not you... who knows?", 2);
			Assert.That(words[0], Is.EqualTo(Tuple.Create("you", 2)));
			Assert.That(words[1], Is.EqualTo(Tuple.Create("knows", 1)));

			words = GetMostFrequentWords("", 100500);
			Assert.That(words, Is.Empty);
		}
	}
}