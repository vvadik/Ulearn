using System;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace uLearn.Courses.Linq.Slides
{
	[Title("Задача №5")]
	[TestFixture]
	public class SortExercise
	{
		/*

		##Задача: Словарь текста

		Дан текст, нужно составить лексикографически упорядоченный список всех слов, которые встречаются в этом тексте.

		Слова выводить в нижнем регистре.

		### Краткая справка
		  * `IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> items, Func<T, K> keySelector)`
		  * `IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> items, Func<T, K> keySelector)`
		  * `IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> items, Func<T, K> keySelector)`
		  * `IOrderedEnumerable<T> ThenByDescending<T>(this IOrderedEnumerable<T> items, Func<T, K> keySelector)`
		*/

		[Exercise(SingleStatement = true)]
		[Hint("`Regex.Split` — позволяет задать регулярное выражение для разделителей слов и получить список слов.")]
		[Hint("`Regex.Split(s, @\"\\W+\")` разбивает текст на слова")]
		[Hint("Пустая строка не является корректным словом")]
		[Hint("У класса `string` есть метод `ToLower` для приведения строки к нижнему регистру")]
		[Hint("Подумайте, как скомбинировать SelectMany, со вложенным Regex.Split")]
		public static string[] GetSortedWords(params string[] textLines)
		{
			return textLines.SelectMany(
				line => Regex.Split(line, @"\W+")
					.Where(word => word != "")
					.Select(word => word.ToLower())
				)
				.Distinct()
				.OrderBy(word => word)
				.ToArray();
			// ваше решение
		}

		[ShowOnSlide]
		[ExpectedOutput("Good")]
		public static void Main()
		{
			var words = GetSortedWords(
				"Hello, hello, hello, how low",
				"",
				"With the lights out, it's less dangerous",
				"Here we are now; entertain us",
				"I feel stupid and contagious",
				"Here we are now; entertain us",
				"A mulatto, an albino, a mosquito, my libido...",
				"Yeah, hey");
			var result = IsRightAnswer(words) ? "Good" : "Bad";
			Console.WriteLine(result);
		}

		public static bool IsRightAnswer(string[] words)
		{
			var rightAnswer = new[]
			{
				"a", "albino", "an", "and", "are", "contagious", "dangerous",
				"entertain", "feel", "hello", "here", "hey", "how",
				"i", "it", "less", "libido", "lights", "low",
				"mosquito", "mulatto", "my", "now", "out", "s", "stupid",
				"the", "us", "we", "with", "yeah"
			};
			if (words.Length == rightAnswer.Length)
			{
				if (rightAnswer.Where((t, i) => words[i] != t).Any())
				{
					return false;
				}
			}
			else return false;
			return true;
		}
	}
}