using System;
using System.Linq;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U04_Arrays
{
	/*
	Напишите метод, который создает массив из count четных чисел больших нуля, в порядке возрастания.
	*/
	[Slide("Четный массив", "{CEB81EAC-069C-4D5D-87B7-EBD9B140E143}")]
	class S015_EvenArray
	{
		[ExpectedOutput(@"
GetFirstEvenNumbers(30)
2 4 6 8 10 12 14 16 18 20 22 24 26 28 30 32 34 36 38 40 42 44 46 48 50 52 54 56 58 60
GetFirstEvenNumbers(0)

GetFirstEvenNumbers(1)
2
")]
		[HideOnSlide]
		public static void Main()
		{
			Run(30);
			Run(0);
			Run(1);
		}

		[HideOnSlide]
		private static void Run(int count)
		{
			Console.WriteLine("GetFirstEvenNumbers({0})", count);
			Console.WriteLine(string.Join(" ", GetFirstEvenNumbers(count).Select(x => x.ToString()).ToArray()));
		}

		[Exercise]
		public static int[] GetFirstEvenNumbers(int count)
		{
			var res = new int[count];
			for (int i = 0; i < count; i++)
				res[i] = 2*(i + 1);
			return res;
		}
	}
}
