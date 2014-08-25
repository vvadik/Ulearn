using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U04_Arrays
{
	[Slide("Индекс максимума", "{043015A0-0B28-4435-8079-21E4CA8E6526}")]
	class S016_FindMinIndex
	{
		/*
		Чтобы освоиться с массивами, вы с Васей решили потренироваться на простых алгоритмах.
		Вася написал метод поиска минимума в массиве:
		*/

		static double Min(double[] array)
		{
			var min = double.MaxValue;
			foreach (var item in array)
				if (item < min) min = item;
			return min;
		}

		/*
		А вам выпала задача посложнее — написать метод поиска индекса максимального элемента.
		
		Если в массиве максимальный элемент встречается несколько раз, вывести нужно минимальный индекс.
		
		Если массив пуст, вывести нужно -1.
		*/

		[ExpectedOutput(@"
2
-1
0
1
3
0
")]
		[HideOnSlide]
		public static void Main()
		{
			Console.WriteLine(MaxIndex(new double[] { 1, 2, 3 }));
			Console.WriteLine(MaxIndex(new double[] { }));
			Console.WriteLine(MaxIndex(new double[] { 1 }));
			Console.WriteLine(MaxIndex(new double[] { 0, 100, 1, 2, 100 }));
			Console.WriteLine(MaxIndex(new double[] { 1, 2, 3, 100, 4, 5, 6 }));
			Console.WriteLine(MaxIndex(new double[] { 100, 100, 100, 100 }));
		}

		[Exercise]
		public static int MaxIndex(double[] a)
		{
			if (a.Length == 0) return -1;
			int maxIndex = 0;
			for (int i = 0; i < a.Length; i++)
				if (a[i] > a[maxIndex]) maxIndex = i;
			return maxIndex;
		}
	}
}
