using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U04_Arrays
{
	[Slide("Найти в массиве", "{94B9A357-8F8F-4E6A-9497-C812A751F091}")]
	class S018_FindPattern
	{
		/*
		Вася решил усложнить вашу задачу и теперь в массиве нужно найти не один элемент, а целый подмассив!

		Если подмассив найден, то вернуть нужно минимальный индекс, с которого начинается подмассив в исходном массиве.

		Если не найден, вернуть нужно -1.

		Считайте, что пустой подмассив содержится в любом массиве, начиная с индекса 0.
		*/

		[ExpectedOutput(@"
0
1
1
0
0
-1
-1
0
0
")]
		[HideOnSlide]
		public static void Main()
		{
			Console.WriteLine(FindIndex(new[] { 100, 100, 100, 100 }, new[] { 100, 100 }));
			Console.WriteLine(FindIndex(new[] { 1, 2, 3 }, new[] { 2 }));
			Console.WriteLine(FindIndex(new[] { 1, 2, 3 }, new[] { 2, 3 }));
			Console.WriteLine(FindIndex(new[] { 1, 2, 3 }, new[] { 1, 2 }));
			Console.WriteLine(FindIndex(new[] { 1, 2, 3 }, new[] { 1, 2, 3 }));
			Console.WriteLine(FindIndex(new[] { 1 }, new[] { 1, 2, 3 }));
			Console.WriteLine(FindIndex(new int[0], new[] { 1, 2, 3 }));
			Console.WriteLine(FindIndex(new[] { 1, 2, 3 }, new int[0]));
			Console.WriteLine(FindIndex(new int[0], new int[0]));
		}

		[Exercise]
		[Hint("Кажется, одним циклом тут не обойтись")]
		[Hint("Напишите сначала вспомогательный метод, который проверяет, входит ли подмассив в массив начиная с заданного индекса.")]
		public static int FindIndex(int[] array, int[] subArray)
		{
			for (int i = 0; i < array.Length - subArray.Length + 1; i++)
				if (ContainsAtIndex(array, subArray, i)) return i;
			return -1;
		}
		
		[ExcludeFromSolution]
		[HideOnSlide]
		private static bool ContainsAtIndex(int[] array, int[] subArray, int startIndex)
		{
			for (int i = 0; i < subArray.Length; i++)
				if (startIndex + i >= array.Length || array[startIndex + i] != subArray[i])
					return false;
			return true;
		}
	}
}
