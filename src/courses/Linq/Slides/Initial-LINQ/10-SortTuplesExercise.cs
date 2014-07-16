using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace uLearn.Courses.Linq.Slides
{
	[Title("Задача №6")]
	[TestFixture]
	public class SortTuples
	{
		/*
		##Сравнение кортежей

		Ещё одно полезное свойство кортежей — по умолчанию они сравниваются поэлементно.
		Используя этот факт, решите следующую задачу:

		##Задача: Словарь текста 2

		Дан текст, нужно составить список всех встречающихся в тексте слов, 
		упорядоченный сначала по длинне слова, а потом лексикографически.

		Запрещено использовать `ThenBy` и `ThenByDescending`.
		*/

		[Exercise(SingleStatement = true)]
		[Hint("`Regex.Split` — позволяет задать регулярное выражение для разделителей слов и получить список слов.")]
		[Hint("`Regex.Split(s, @\"\\W+\")` разбивает текст на слова")]
		[Hint("Пустая строка не является корректным словом")]
		[Hint("`keySelector` в `OrderBy` должен возвращать ключ сортировки. Этот ключ может быть кортежем.")]
		public static List<string>  GetSortedWords(string text)
		{
			return Regex.Split(text, @"\W+")
				.Where(word => word != "")
				.Distinct()
				.OrderBy(word => Tuple.Create(word.Length, word))
				.ToList();
			// ваше решение
		}

		[ExpectedOutput("Good")]
		[ShowOnSlide]
		public static void Main()
		{
			var sortedList = GetSortedWords("you?! or not you... who knows?");
			var result = IsRightAnswer(sortedList) ? "Good" : "Bad";
			Console.WriteLine(result);
		}

		public static bool IsRightAnswer(List<string> sortedWords )
		{
			var rightAnswer = new[] {"or", "not", "who", "you", "knows"};
			if (sortedWords.Count != rightAnswer.Length)
				return false;
			return !sortedWords.Where((t, i) => t != rightAnswer[i]).Any();
		}
	}
}