using System;
using NUnit.Framework;

namespace uLearn.Courses.BasicProgramming.Slides.U09_Sorting
{
	[Slide("Быстрая сортировка", "E9592A21-70D0-4623-99FF-1DA9CDE1B691")]
	public class S070_QuickSort
	{
		//#video hwvFP2XLGWk
		/*
		## Заметки по лекции

		Быстрая сортировка (Quick-Sort) и сортировка Хоара (Hoare-Sort) — это синонимы.

		Интересный факт: она была изобретена Чарльзом Хоаром во время его работы в МГУ в 1960 году.
		*/

		static void HoareSort(int[] array, int start, int end)
		{
			if (end == start) return;
			var pivot = array[end];
			var storeIndex = start;
			for (int i = start; i <= end - 1; i++)
				if (array[i] <= pivot)
				{
					var t = array[i];
					array[i] = array[storeIndex];
					array[storeIndex] = t;
					storeIndex++;
				}

			var n = array[storeIndex];
			array[storeIndex] = array[end];
			array[end] = n;
			if (storeIndex > start) HoareSort(array, start, storeIndex - 1);
			if (storeIndex < end) HoareSort(array, storeIndex + 1, end);
		}

		static void HoareSort(int[] array)
		{
			HoareSort(array, 0, array.Length - 1);
		}


		static Random random = new Random();

		static int[] GenerateArray(int length)
		{
			var array = new int[length];
			for (int i = 0; i < array.Length; i++)
				array[i] = random.Next();
			return array;
		}
		
		[Test]
		public static void Main()
		{
			var array = GenerateArray(10);
			HoareSort(array);
			foreach (var e in array)
				Console.WriteLine(e);
		}

		/*
		![quicksort](quicksort.gif)
		*/
	}
}