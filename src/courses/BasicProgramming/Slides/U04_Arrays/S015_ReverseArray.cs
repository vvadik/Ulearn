using System;
using System.Linq;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U04_Arrays
{
	/*
	Напишите метод, который создает перевернутую копию переданного ему массива. Первый элемент должен стать последним, второй предпоследним и так далее.
	*/
	[Slide("Перевернуть массив", "{D9741319-FBDF-448E-BC4F-DB17CDC21B21}")]
	class S015_ReverseArray
	{
		[ExpectedOutput(@"
Reverse([1, 2, 3]) = [3, 2, 1]
Reverse([1, 2, 3, 4]) = [4, 3, 2, 1]
Reverse([1]) = [1]
Reverse([]) = []
Reverse([1, 1, 1, 2, 2, 2, 3]) = [3, 2, 2, 2, 1, 1, 1]
")]
		[HideOnSlide]
		public static void Main()
		{
			Check(new[] {1, 2, 3});
			Check(new[] {1, 2, 3, 4});
			Check(new[] {1});
			Check(new int[0]);
			Check(new[]{1, 1, 1, 2, 2, 2, 3});
		}

		[HideOnSlide]
		private static void Check(int[] array)
		{
			var reversed = Reverse(array);
			if (array == reversed)
				Console.WriteLine("Original array should not be changed!");
			else
			{
				Console.WriteLine("Reverse([{0}]) = [{1}]",
					string.Join(", ", array.Select(a => a.ToString()).ToArray()),
					string.Join(", ", reversed.Select(a => a.ToString()).ToArray())
					);
			}
		}

		[Exercise]
		public static int[] Reverse(int[] array)
		{
			var reversed = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
				reversed[i] = array[array.Length - i - 1];
			return reversed;
			/*uncomment
			...
			*/
		}
	}
}
