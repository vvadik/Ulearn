using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U03_Cycles
{
	[Slide("Среднее трех", "{937C4E64-7144-4F52-A75D-4BDC95BBDE72}")]
	class S022_MiddleOf3Task
	{
		/*
		А теперь Васе очень хочется находить среднее по величине из трех данных чисел.
		Он начал писать код, но абсолютно запутался в ветках if-а! Помогите ему.
		*/

		[ExpectedOutput(@"
5
12
2
8
1
2
2
2
2
2
2
1
1
1
1
")]
		public static void Main()
		{
			Console.WriteLine(MiddleOf(5, -1, 555)); // => 5
			Console.WriteLine(MiddleOf(12, 12, 11));
			Console.WriteLine(MiddleOf(2, 3, 2));
			Console.WriteLine(MiddleOf(8, 8, 8));
			Console.WriteLine(MiddleOf(5, 0, 1)); 
			Console.WriteLine(MiddleOf(1, 2, 3));
			Console.WriteLine(MiddleOf(1, 3, 2));
			Console.WriteLine(MiddleOf(2, 1, 3));
			Console.WriteLine(MiddleOf(2, 3, 1));
			Console.WriteLine(MiddleOf(3, 1, 2));
			Console.WriteLine(MiddleOf(3, 2, 1));
			Console.WriteLine(MiddleOf(1, 1, 2));
			Console.WriteLine(MiddleOf(1, 2, 1));
			Console.WriteLine(MiddleOf(2, 1, 1));
			Console.WriteLine(MiddleOf(1, 1, 1));

		}

		[Exercise]
		[Hint("Успехов")]
		[Hint("Ну какие тут подсказки?!")]
		[Hint("Просто напишите этот код!")]
		private static int MiddleOf(int a, int b, int c)
		{
			if (a > b)
				if (b > c) return b;
				else if (a > c) return c;
				else return a;
			if (a > c) return a;
			if (b > c) return c;
			return b;
			/*uncomment
			...
			*/
		}
	}
}
