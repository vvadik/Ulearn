using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using uLearn.CSharp;

namespace uLearn.Courses.Linq.Slides.LINQ
{
	[Slide("Сортировка кортежей", "{80D43879-1099-4972-AEE1-6EB3EDF1E923}")]
	public class S100_SortTuplesExercise : SlideTestBase
	{
		/*
		Еще одно полезное свойство кортежей — они реализуют интерфейс `IComparable`, 
		сравнивающий кортежи по компонентам. 
		То есть `Tuple.Create(1, 2)` будет меньше `Tuple.Create(2, 1)`.
		Этот интерфейс по умолчанию используется в методах сортировки и поиска минимума и максимума.

		Используя этот факт, решите следующую задачу.

		Дан текст, нужно составить список всех встречающихся в тексте слов, 
		упорядоченный сначала по возрастанию длины слова, а потом лексикографически.

		Запрещено использовать `ThenBy` и `ThenByDescending`.
		*/

		[Exercise]
		[SingleStatementMethod]
		[Hint("`Regex.Split` — позволяет задать регулярное выражение для разделителей слов и получить список слов.")]
		[Hint("`Regex.Split(s, @\"\\W+\")` разбивает текст на слова")]
		[Hint("Пустая строка не является корректным словом")]
		[Hint("`keySelector` в `OrderBy` должен возвращать ключ сортировки. Этот ключ может быть кортежем.")]
		public static List<string>  GetSortedWords(string text)
		{
			return Regex.Split(text.ToLower(), @"\W+")
				.Where(word => word != "")
				.Distinct()
				.OrderBy(word => Tuple.Create(word.Length, word))
				.ToList();
			// ваше решение
		}

		[ExpectedOutput(@"
GetSortedWords(""A box of biscuits, a box of mixed biscuits, and a biscuit mixer."")
'a' 'of' 'and' 'box' 'mixed' 'mixer' 'biscuit' 'biscuits'

GetSortedWords("""")


GetSortedWords(""Each Easter Eddie eats eighty Easter eggs."")
'each' 'eats' 'eggs' 'eddie' 'easter' 'eighty'
")]
		[HideOnSlide]
		public static void Main()
		{
			CheckOn("A box of biscuits, a box of mixed biscuits, and a biscuit mixer.");
			CheckOn("");
			CheckOn("Each Easter Eddie eats eighty Easter eggs.");
		}

		[HideOnSlide]
		private static void CheckOn(string text)
		{
			Console.WriteLine("GetSortedWords(\"{0}\")", text);
			var sortedList = GetSortedWords(text);
			Console.WriteLine(string.Join(" ", sortedList.Select(w => "'" + w + "'").ToArray()));
			Console.WriteLine();
		}
	}
}