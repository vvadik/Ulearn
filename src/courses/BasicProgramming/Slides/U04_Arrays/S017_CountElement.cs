using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U04_Arrays
{
	[Slide("Подсчет", "{9EB1A5C2-135D-49A5-A922-0F3F91566080}")]
	class S017_CountElement
	{
		/*
		Тренировки продолжаются.
		На этот раз Вася написал метод, подсчитывающий количество чётных чисел в массиве.
		*/

		static int GetEvenCount(int[] array)
		{
			var count = 0;
			foreach (var item in array)
				if (item % 2 == 0) count++;
			return count;
		}

		/*
		А вас ждет задача поиска количества вхождений в массив заданного числа.
		*/

		[ExpectedOutput(@"
1
0
1
2
0
4
")]
		[HideOnSlide]
		public static void Main()
		{
			Console.WriteLine(GetElementCount(new [] { 1, 2, 3 }, 1));
			Console.WriteLine(GetElementCount(new int[] { }, 42));
			Console.WriteLine(GetElementCount(new [] { 1 }, 1));
			Console.WriteLine(GetElementCount(new [] { 0, 100, 1, 2, 100 }, 100));
			Console.WriteLine(GetElementCount(new [] { 1, 2, 3, 100, 4, 5, 6 }, 42));
			Console.WriteLine(GetElementCount(new [] { 100, 100, 100, 100 }, 100));
		}

		[Exercise]
		public static int GetElementCount(int[] items, int itemToFind)
		{
			var count = 0;
			foreach (var item in items)
				if (item == itemToFind) count++;
			return count;
		}
	}
}
