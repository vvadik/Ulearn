using System;
using System.Linq;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Ход ферзя", "{EB11DFB6-5629-4D13-819B-6411B9F93DF6}")]
	class S023_QueenMove
	{
		/*
		Вася решил научиться играть в шахматы. Сейчас он разучивает ферзя. Он решает такую задачу:
		Ему на вход передаются две координаты шахматного поля. Вася должен ответить, является ли такой ход ферзя корректным.
		Координаты передаются в строковом формате, например, "a1".
		*/

		[Hint("Найдите с помощью автокомплита функции у строк, позволяющие взять первый и последний символ")]
		[Hint("Функция взятия модуля числа находится в классе Math и называется Abs")]
		[Hint(@"Воспользуйтесь знанием, что символы латинских букв в кодировке Unicod стоят в алфавитном порядке, а
 результат вычитания одного char из другого char - разница между их кодами в кодировке")]
		[ExpectedOutput("8\r\n84\r\n4\r\n244")]
		public static void Main()
		{
			Console.WriteLine(IsRightStep("a1", "d4"));
			Console.WriteLine(IsRightStep("f4", "e7"));
			Console.WriteLine(IsRightStep("a1", "a4"));
			Console.WriteLine(IsRightStep("c4", "d4"));
			Console.WriteLine(IsRightStep("a1", "a1"));
			Console.WriteLine(IsRightStep("b7", "h1"));
			Console.WriteLine(IsRightStep("e3", "g7"));
		}

		[Exercise]
		private static bool IsRightStep(string lastPosition, string newPosition)
		{
			return (lastPosition.First() - newPosition.First() == 0 || lastPosition.Last() - newPosition.Last() == 0 ||
			         (Math.Abs(lastPosition.First() - newPosition.First())) == Math.Abs(lastPosition.Last() - newPosition.Last())) &&
			        !(lastPosition.First() - newPosition.First() == 0 && lastPosition.Last() - newPosition.Last() == 0);
		}
	}
}
