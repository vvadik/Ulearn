using System;
using System.Collections.Generic;
using System.Linq;

namespace uLearn.Courses.Linq.Slides
{
	[Slide("Поиск самого длинного слова", "{04A35AE5-67B9-4674-B4C5-98E8976F87F9}")]
	public class AggregateExercise
	{
		/*

		Дан список слов, нужно найти самое длинное слово из этого списка, 
		а из всех самых длинных — лексикографически первое слово.

		Решите эту задачу в одно выражение. 
		
		Не используйте методы сортировки — сложность сортировки O(N * log(N)), однако эту задачу можно решить за O(N).

		*/

		[Exercise(SingleStatement = true)]
		[Hint("Вспомните про кортежи")]
		[Hint("Вспомните про особенности сравнения кортежей")]
		public static string GetLongest(IEnumerable<string> words)
		{
			return words.Min(line => Tuple.Create(-line.Length, line)).Item2;
			//Ваш код
		}

		[ExpectedOutput("azaz\nsdsd\n12345")]
		public static void Main()
		{
			Console.WriteLine(GetLongest(new[] {"azaz", "as", "sdsd"}));
			Console.WriteLine(GetLongest(new[] {"zzzz", "as", "sdsd"}));
			Console.WriteLine(GetLongest(new[] {"as", "12345", "as", "sds"}));
		}
	}
}