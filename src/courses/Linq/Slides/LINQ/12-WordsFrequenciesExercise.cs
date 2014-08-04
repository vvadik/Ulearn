using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace uLearn.Courses.Linq.Slides
{
	[Slide("Создание частотного словаря", "{0535734D-D258-44C6-99F3-F96258BCCA6F}")]
	public class GroupingExercise
	{
		/*

		Дан текст, нужно вывести `count` наиболее часто встречающихся в тексте слов, вместе с их частотой.
		Среди слов, встречающихся одинаково часто, отдавать предпочтение лексикографически меньшим словам.
		Слова сравнивать регистронезависимо и выводить в нижнем регистре.		
		
		Напомним сигнатуры некоторых Linq-методов, которые могут понадобятся в этом упражнении:

		    IEnumerable<IGrouping<K, T>>       GroupBy(this IEnumerable<T> items, Func<T, K> keySelector)
		    IOrderedEnumerable<T>           OrderBy<T>(this IEnumerable<T> items, Func<T, K> keySelector)
		    IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> items, Func<T, K> keySelector)
		    IOrderedEnumerable<T>            ThenBy<T>(this IOrderedEnumerable<T> items, Func<T, K> keySelector)
		    IOrderedEnumerable<T>  ThenByDescending<T>(this IOrderedEnumerable<T> items, Func<T, K> keySelector)
		    IEnumerable<T>                        Take(this IEnumerable<T> items, int count)
		*/

		[Exercise(SingleStatement = true)]
		public static Tuple<string, int>[] GetMostFrequentWords(string text, int count)
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

		[ExpectedOutput(@"
GetMostFrequentWords(""A box of biscuits, a box of mixed biscuits, and a biscuit mixer."", 2)
  a 3
  biscuits 2

GetMostFrequentWords("""", 100)


GetMostFrequentWords(""Each Easter Eddie eats eighty Easter eggs."", 3)
  easter 2
  each 1
  eats 1")]
		[HideOnSlide]
		public static void Main()
		{
			CheckOn(2, "A box of biscuits, a box of mixed biscuits, and a biscuit mixer.");
			CheckOn(100, "");
			CheckOn(3, "Each Easter Eddie eats eighty Easter eggs.");
		}

		[HideOnSlide]
		private static void CheckOn(int count, string text)
		{
			Console.WriteLine("GetMostFrequentWords(\"{0}\", {1})", text, count);
			var sortedList = GetMostFrequentWords(text, count);
			Console.WriteLine(string.Join(Environment.NewLine, sortedList.Select(t => "  " + t.Item1 + " " + t.Item2).ToArray()));
			Console.WriteLine();
		}
	}
}