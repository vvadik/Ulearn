using System;
using NUnit.Framework;

namespace uLearn.Courses.BasicProgramming.Slides.U09_Sorting
{
	[Slide("Бинарный поиск", "E7E0D77F-8D3F-418D-9526-810BC59C792D")]
	public class S010_BinSearch
	{
		//#video nX2QeYilOd8
		/*
		## Заметки по лекции
		*/

		static int FindIndex(int[] array, int element)
		{
			for (int i = 0; i < array.Length; i++)
				if (array[i] == element) return i;
			return -1;
		}

		static int FindIndexByBinarySearch(int[] array, int element)
		{
			var left = 0;
			var right = array.Length - 1;
			while (left < right)
			{
				var middle = (right + left) / 2;
				if (element <= array[middle])
					right = middle;
				else left = middle + 1;
			}
			if (array[right] == element)
				return right;
			return -1;
		}

		[Test]
		public static void Main()
		{
			var array = new[]{1,2,3,4,5,5,5,6};
			Console.WriteLine(FindIndex(array, 5));
			Console.WriteLine(FindIndexByBinarySearch(array, 5));
		}
	}
}