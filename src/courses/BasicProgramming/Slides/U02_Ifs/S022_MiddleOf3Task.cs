using System;
using System.Linq;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U03_Cycles
{
	[Slide("Среднее трех", "{937C4E64-7144-4F52-A75D-4BDC95BBDE72}")]
	class S022_MiddleOf3Task
	{
		/*
		Вы с Васей и Петей решили выбрать самые лучшие фотографии котиков в интернете.
		Для этого каждую фотографию каждый из вас оценил по сто-бальной шкале.
		Естественно тут же встал вопрос о том, как из трех оценок получить одну финальную.

		Вы опасаетесь, что каждый из вас сильно завысит оценку фотографиям своего котика. 
		Поэтому было решено в качестве финальной оценки брать не среднее арифметическое, 
		а медиану, то есть просто откинуть максимальную и минимальную оценки.

		Вася хотел сам написать код выбора средней оценки из трех, но быстро запутался в if-ах, поэтому перепоручил эту задачу вам.

		Попробуйте не просто решить задачу, но и минимизировать количество проверок и максимально упростить условия проверок.

		Если у вас быстро получится решить эту задачу с помощью if-ов, попробуйте подумать над более элегантными и хитроумными решениями.
		*/

		[ExpectedOutput(@"
5
12
2
8
1
")]
		public static void Main()
		{
			Console.WriteLine(MiddleOf(5, 0, 100)); // => 5
			Console.WriteLine(MiddleOf(12, 12, 11)); // => 12
			Console.WriteLine(MiddleOf(2, 3, 2));
			Console.WriteLine(MiddleOf(8, 8, 8));
			Console.WriteLine(MiddleOf(5, 0, 1));

			FinalTesting();

		}

		[HideOnSlide]
		private static void FinalTesting()
		{
			var rnd = new Random();
			var errors = 
				from iteration in Enumerable.Range(0, 5)
				let ar = new[] {rnd.Next(0, 100), rnd.Next(0, 100), rnd.Next(0, 100)}
				from a in Enumerable.Range(0, 3).Select(i => ar[i])
				from b in Enumerable.Range(0, 3).Select(i => ar[i])
				from c in Enumerable.Range(0, 3).Select(i => ar[i])
				let expectedMiddle = new[] {a, b, c}.OrderBy(x => x).ElementAt(1)
				where MiddleOf(a, b, c) != expectedMiddle
				select string.Format("FinalTesting failed on [{0}, {1}, {2}]", a, b, c);
			foreach (var error in errors)
			{
				Console.WriteLine(error);
				return;
			}
		}

		[Exercise]
		[Hint("Не запутайтесь в if-ах. Придумайте какую-нибудь систему и придерживайтесь ее.")]
		public static int MiddleOf(int a, int b, int c)
		{
			if (a > b)
				if (b > c) return b;
				else if (a > c) return c;
				else return a;
			if (a > c) return a;
			if (b > c) return c;
			return b;
			/*uncomment
			if (a > b)
				if (a > c) ...
			*/
		}
	}
}
