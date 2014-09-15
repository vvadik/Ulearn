using System;
using System.Collections.Generic;
using System.Linq;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Ход ферзя", "{EB11DFB6-5629-4D13-819B-6411B9F93DF6}")]
	internal class S023_QueenMove
	{
		/*
		Вася решил научить своего младшего брата играть в шахматы. 
		Но вот беда, брат еще слишком мал и никак не может запомнить как ходит ферзь.
		Как настоящий программист, Вася решил автоматизировать ручной труд по обучению начинающих шахматистов.
		
		Помогите ему написать программу, которая по шахматному ходу определяет корректный ли это ход ферзя.
		Координаты исходной и конечной позиции хода передаются строкой в обычной шахматной нотации, например, "a1".

		Как обычно, постарайтесь поразить Васю красотой и простотой получившегося кода!
		*/

		[Hint(@"Значение `char` — это номер буквы (то есть число) в некоторой таблице кодировки. Латинские буквы в этой кодировке идут подряд в алфавитном порядке.")]
		[Hint(@"Учитывая предыдущие подсказки, символы можно вычитать. Например `'c' - 'a' == 2`")]
		[Hint(@"Функция взятия модуля числа находится в классе `Math` и называется `Abs`. С ней код может стать проще!")]
		[ExpectedOutput(@"
a1-d4 True
f4-e7 False
a1-a4 True
")]
		[CommentAfterExerciseIsSolved("Если догадаться обозначить за dx и dy модули смещения ферзя по горизонтали и вертикали за свой ход, то условие корректности кода можно записать предельно компактно и красиво.")]
		public static void Main()
		{
			TestMove("a1", "d4");
			TestMove("f4", "e7");
			TestMove("a1", "a4");
			FinalTesting();
		}

		[HideOnSlide]
		private static void FinalTesting()
		{
			foreach (var error in GetAllErrors())
			{
				Console.WriteLine(error);
				return;
			}
		}

		[HideOnSlide]
		private static IEnumerable<string> GetAllErrors()
		{
			Func<int, int, string> formatPos = (x, y) => "" + ("abcdefgh"[x]) + (y+1);
			return
				from x1 in Enumerable.Range(0, 8)
				from y1 in Enumerable.Range(0, 8)
				from x2 in Enumerable.Range(0, 8)
				from y2 in Enumerable.Range(0, 8)
				let dx = Math.Abs(x1 - x2)
				let dy = Math.Abs(y1 - y2)
				let pos1 = formatPos(x1, y1)
				let pos2 = formatPos(x2, y2)
				let isCorrect = dx + dy != 0 && (dx == dy || dx*dy == 0)
				where IsCorrectMove(pos1, pos2) != isCorrect
				select string.Format("FinalTesting failed on {0}-{1}", pos1, pos2);
		}

		public static void TestMove(string from, string to)
		{
			Console.WriteLine("{0}-{1} {2}", from, to, IsCorrectMove(from, to));
		}

		[Exercise]
		public static bool IsCorrectMove(string from, string to)
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