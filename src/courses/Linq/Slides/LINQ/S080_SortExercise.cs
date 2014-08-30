using System;
using System.Linq;
using System.Text.RegularExpressions;
using uLearn.CSharp;

namespace uLearn.Courses.Linq.Slides
{
	[Slide("Составление словаря", "{ACB110B3-C2F0-4E1A-9645-76DF88A75A7F}")]
	public class S080_SortExercise
	{
		/*

		Текст задан массивом строк. 
		Вам нужно составить лексикографически упорядоченный список всех встречающихся в этом тексте слов.

		Слова нужно сравнивать регистронезависимо, а выводить в нижнем регистре.
		*/

		[Exercise]
		[SingleStatementMethod]
		[Hint("`Regex.Split` — позволяет задать регулярное выражение для разделителей слов и получить список слов.")]
		[Hint("`Regex.Split(s, @\"\\W+\")` разбивает текст на слова")]
		[Hint("Подумайте, как скомбинировать SelectMany, со вложенным `Regex.Split`")]
		[Hint("Пустая строка не является корректным словом")]
		[Hint("У класса `string` есть метод `ToLower` для приведения строки к нижнему регистру")]
		[Hint("`list.Distinct()` — возвращает список `list` без дубликатов")]
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

		[ExpectedOutput(@"a
albino
an
and
are
contagious
dangerous
entertain
feel
hello
here
hey
how
i
it
less
libido
lights
low
mosquito
mulatto
my
now
out
s
stupid
the
us
we
with
yeah")]
		public static void Main()
		{
			var vocabulary = GetSortedWords(
				"Hello, hello, hello, how low",
				"",
				"With the lights out, it's less dangerous",
				"Here we are now; entertain us",
				"I feel stupid and contagious",
				"Here we are now; entertain us",
				"A mulatto, an albino, a mosquito, my libido...",
				"Yeah, hey"
			);
			foreach (var word in vocabulary)
				Console.WriteLine(word);
		}
	}
}