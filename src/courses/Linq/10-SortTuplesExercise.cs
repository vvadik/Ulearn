using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace SharpLessons.Linqed
{
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
		public List<string> GetSortedWords(string text)
		{
			return Regex.Split(text, @"\W+")
				.Where(word => word != "")
				.Distinct()
				.OrderBy(word => Tuple.Create(word.Length, word))
				.ToList();
			// ваше решение
		}

		[Test]
		public void Test()
		{
			var words = GetSortedWords("you?! or not you... who knows?");
			Assert.That(words, Is.EqualTo(new[] {"or", "not", "who", "you", "knows"}));
		}
	}
}