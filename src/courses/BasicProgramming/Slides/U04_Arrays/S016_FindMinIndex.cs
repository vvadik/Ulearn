  using System;
  using System.Linq;
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
		То есть такого числа i, что array[i] — это максимальное из чисел в массиве.
		
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
999999
0
")]
		public static void Main()
		{
			Console.WriteLine(MaxIndex(new double[] { 1, 2, 3 }));
			Console.WriteLine(MaxIndex(new double[] { }));
			Console.WriteLine(MaxIndex(new double[] { 1 }));
			Console.WriteLine(MaxIndex(new double[] { 0, 100, 1, 2, 100 }));
			Console.WriteLine(MaxIndex(new double[] { 1, 2, 3, 100, 4, 5, 6 }));
			Console.WriteLine(MaxIndex(new double[] { 100, 100, 100, 100 }));
			Console.WriteLine(MaxIndex(CreateSecretBigArray1()));
			Console.WriteLine(MaxIndex(CreateSecretBigArray2()));
		}
		[HideOnSlide]
		private static double[] CreateSecretBigArray1()
		{
			return Enumerable.Range(1, 1000000).Select(i => (double)i).ToArray();
		}
		[HideOnSlide]
		private static double[] CreateSecretBigArray2()
		{
			return Enumerable.Range(1, 1000000).Select(i => 42.0).ToArray();
		}

		[Exercise]
		[Hint("Помните, что массив может быть пустым")]
		[Hint("Длина массива находится в ```.Length```")]
		public static int MaxIndex(double[] array)
		{
			if (array.Length == 0) return -1;
			int maxIndex = 0;
			for (int i = 0; i < array.Length; i++)
				if (array[i] > array[maxIndex]) maxIndex = i;
			return maxIndex;
		}
	}
}
