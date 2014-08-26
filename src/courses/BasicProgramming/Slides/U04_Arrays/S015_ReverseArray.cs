using System;

using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U04_Arrays
{
	/*
	Напишите метод, который создает массив из 30 первых четных чисел.
	*/
	[Slide("Чётный массив", "{CEB81EAC-069C-4D5D-87B7-EBD9B140E143}")]
	class S015_ReverseArray
	{
		[ExpectedOutput(@"
2 4 6 8 10 12 14 16 18 20 22 24 26 28 30 32 34 36 38 40 42 44 46 48 50 52 54 56 58 60
")]
		[HideOnSlide]
		public static void Main()
		{
			foreach (var num in Get30EvenNumbers())
				Console.Write(num + " ");
		}

		[Exercise]
		public static int[] Get30EvenNumbers()
		{
			var res = new int[30];
			for (int i = 0; i < 30; i++)
				res[i] = 2*(i + 1);
			return res;
		}
	}
}
