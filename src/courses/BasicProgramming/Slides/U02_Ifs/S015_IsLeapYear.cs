using System;
using System.Linq;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Високосный год", "{4C161B1E-2637-447B-ADFD-14647BF659AD}")]
	class S015_IsLeapYear
	{
		/*
		Вы с Васей решили посчитать количество дней между двумя солнечными затмениями. 
		Сначала казалось, что все просто, но потом вы вспомнили про високосные года.
		
		Пока Вася пишет все остальное, напишите для него функцию для определения является ли заданный год високосным.

		В интернете несложно [найти](https://ru.wikipedia.org/wiki/Григорианский_календарь) алгоритм.
		
		Поразите Васю краткостью и лаконичностью — напишите решение в одно логическое выражение, без использования готовых функций.
		*/

		[ExpectedOutput("False\nFalse\nTrue\nFalse\nFalse\nTrue\nFalse\nTrue")]
		[Hint("Вспомните про операцию взятия остатка от деления `%`")]
		[Hint("Нет смысла заводить переменные, пишите выражение сразу после `return`")]
		[CommentAfterExerciseIsSolved("Для учебных целей полезно хотя бы раз написать стандартный алгоритм самостоятельно, однако в реальности всегда нужно стараться использовать готовые библиотечные функции, если они есть. Но для этой задачи есть готовая функция `IsLeapYear` и она предсказуемо находится в классе `DateTime`.")]
		public static void Main()
		{
			Console.WriteLine(IsLeapYear(2014));
			Console.WriteLine(IsLeapYear(1999));
			Console.WriteLine(IsLeapYear(8992));
			Console.WriteLine(IsLeapYear(1));
			Console.WriteLine(IsLeapYear(14));
			Console.WriteLine(IsLeapYear(400));
			Console.WriteLine(IsLeapYear(600));
			Console.WriteLine(IsLeapYear(3200));
			FinalTesting();
		}
		
		[HideOnSlide]
		private static void FinalTesting()
		{
			var random = new Random(12456);
			var badYears =
				Enumerable.Range(1, 3000)
					.OrderBy(year => random.Next())
					.Where(year => IsLeapYear(year) != DateTime.IsLeapYear(year));

			foreach (var year in badYears)
			{
				Console.WriteLine("FinalTesting failed on year {0}", year);
				return;
			}
		}

		[Exercise]
		[SingleStatementMethod]
		public static bool IsLeapYear(int year)
		{
			return year % 4 == 0 && year % 100 != 0 || year % 400 == 0;
			/*uncomment
			return ...
			*/
		}
	}
}
