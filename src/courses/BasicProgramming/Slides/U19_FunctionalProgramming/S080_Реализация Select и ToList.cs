using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn;

namespace U19_FunctionalProgramming_1
{
	[Slide("Реализация Select и ToList", "711daa32-911c-4910-801c-1376bd8f23b3")]
	class S080_Реализация_Select_и_ToList
	{
		//#video 6IFRtXawLSw
		/*
		## Заметки по лекции
		*/
	}
	public class Student
	{
		public string LastName { get; set; }
		public string Group { get; set; }
	}

	public static class IEnumerableExtensions
	{
		public static IEnumerable<T> Where<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
		{
			foreach (var e in enumerable)
				if (predicate(e))
					yield return e;
		}

		public static IEnumerable<TOut> Select<TIn, TOut>(this IEnumerable<TIn> enumerable, Func<TIn, TOut> selector)
		{
			foreach (var e in enumerable)
				yield return selector(e);
		}

		public static List<T> ToList<T>(this IEnumerable<T> enumerable)
		{
			var list = new List<T>();
			foreach (var e in enumerable)
				list.Add(e);
			return list;
		}
	}


	public class Program
	{
		public static void MainX()
		{
			var students = new List<Student>
				{
                new Student { LastName="Jones", Group="FT-1" },
                new Student { LastName="Adams", Group="FT-1" },
                new Student { LastName="Williams", Group="KN-1"},
                new Student { LastName="Brown", Group="KN-1"}
				};

			var result = students.Where(z => z.Group == "KN-1").Select(z => z.LastName).ToList();
		}
	}
}
