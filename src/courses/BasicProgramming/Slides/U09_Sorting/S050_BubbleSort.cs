using System;
using NUnit.Framework;

namespace uLearn.Courses.BasicProgramming.Slides.U09_Sorting
{
	[Slide("Сортировка пузырьком", "71470E4D-756B-4811-8B22-103E020B8024")]
	public class S050_BubbleSort
	{
		//#video VZ0ARjWMAyo
		/*
		## Заметки по лекции
		*/

		private static readonly Random random = new Random();

		private static void BubbleSort(int[] array)
		{
			for (int i = 0; i < array.Length; i++)
				for (int j = 0; j < array.Length - 1; j++)
					if (array[j] > array[j + 1])
					{
						int t = array[j + 1];
						array[j + 1] = array[j];
						array[j] = t;
					}
		}

		[Test]
		public static void Main()
		{
			int[] array = GenerateArray(10);
			BubbleSort(array);
			foreach (int e in array)
				Console.WriteLine(e);
		}

		private static int[] GenerateArray(int length)
		{
			var array = new int[length];
			for (int i = 0; i < array.Length; i++)
				array[i] = random.Next();
			return array;
		}

		/*
		![bubblesort](bubblesort.gif)
		*/
	}
}