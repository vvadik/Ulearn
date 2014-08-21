using System;
using System.Linq;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Ход ферзя", "{EB11DFB6-5629-4D13-819B-6411B9F93DF6}")]
	internal class S023_QueenMove
	{
		/*
		Вася решил научить своего младшего брата играть в шахматы. Но вот беда, брат никак не может запомнить как ходит ферзь.
		Как настоящий программист Вася решил автоматизировать ручной труд по обучению начинающих шахматистов.

		Помогите ему написать программу, которая по шахматному ходу определяет корректный ли это ход ферзя.
		Координаты исходной и конечной позиции хода передаются строкой в обычной шахматной нотации, например, "a1".
		*/

		[Hint("Найдите с помощью автокомплита функции у строк, позволяющие взять первый и последний символ")]
		[Hint("Функция взятия модуля числа находится в классе Math и называется Abs")]
		[Hint(@"Воспользуйтесь знанием, что символы латинских букв в кодировке Unicode стоят в алфавитном порядке, а
 результат вычитания одного char из другого char - разница между их кодами в кодировке")]
		[ExpectedOutput(@"
a1-d4 True
f4-e7 False
a1-a4 True
c4-d4 True
a1-a1 False
b7-h1 True
e3-g7 False
")]
		public static void Main()
		{
			Run("a1", "d4");
			Run("f4", "e7");
			Run("a1", "a4");
			Run("c4", "d4");
			Run("a1", "a1");
			Run("b7", "h1");
			Run("e3", "g7");
		}

		public static void Run(string from, string to)
		{
			Console.WriteLine("{0}-{1} {2}", from, to, IsRightStep(from, to));
		}

		[Exercise]
		private static bool IsRightStep(string from, string to)
		{
			var dx = Math.Abs(to[0] - from[0]);
			var dy = Math.Abs(to[1] - from[1]);
			return dx + dy != 0 && (dx == dy || dx*dy == 0);
			/*uncomment
			...
			*/
		}
	}
}