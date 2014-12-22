using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn;

namespace U19_FunctionalProgramming_0
{
	[Slide("Реализация метода Where", "41db7906-565c-4867-a9ee-e97c89886956")]
	class S070_Реализация_метода_Where
	{
		//#video gvywqLXHutA
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

			var result = students.Where(z => z.Group == "KN-1");//.Select(z => z.LastName).ToList();
		}
	}
}
