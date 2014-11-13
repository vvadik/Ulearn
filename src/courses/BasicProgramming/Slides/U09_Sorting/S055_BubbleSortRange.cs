using System;
using System.Linq;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U09_Sorting
{
	[Slide("Сортировка диапазона", "6C73521D810A429A95F9EADB4F0176E7")]
	public class S055_BubbleSortRange : SlideTestBase
	{
		/*
		А что если отсортировать нужно лишь часть массива?
		Скажем, начиная с индекса left и заканчивая индексом right.

		Доработайте алгоритм сортировки пузырьком так, чтобы он сортировал только указанный диапазон, 
		а элементы за этим диапазоном осталял на своих местах.

		Например, `BubbleSortRange(new[] {4, 3, 2, 1}, 1, 2)` должен вернуть массив `new[] {4, 2, 3, 1}`.
		*/

		[Exercise]
		public static void BubbleSortRange(int[] array, int left, int right)
		{
			for (int i = left; i <= right; i++)
				for (int j = left; j <= right - 1; j++)
					if (array[j] > array[j + 1])
					{
						var t = array[j + 1];
						array[j + 1] = array[j];
						array[j] = t;
					}
			/*uncomment
			for (int i = 0; i < array.Length; i++)
				for (int j = 0; j < array.Length - 1; j++)
					if (array[j] > array[j + 1])
					{
						var t = array[j + 1];
						array[j + 1] = array[j];
						array[j] = t;
					}
			*/
		}

		[ExpectedOutput("")]
		[HideExpectedOutputOnError]
		[HideOnSlide]
		public static void Main()
		{
			CheckSorting(new[] { 4, 2, 3, 1 }, 0, 3);
			CheckSorting(new[] { 1, 3, 2, 4 }, 1, 2);
			CheckSorting(new[] { 60000, 3, 2, 4 }, 1, 2);
			CheckSorting(new[] { 1 }, 0, 0);
			CheckSorting(new[] { int.MaxValue, 3, 2, 4, 2, 5, int.MinValue }, 0, 2);
			CheckSorting(new[] { int.MinValue, 3, 2, 4, 2, 5, int.MinValue }, 0, 2);
			CheckSorting(new[] { int.MaxValue, 3, 2, 4, 2, 5, int.MaxValue }, 0, 2);
			CheckSorting(new[] { 1, 3, 2, 4, 2, 5, 1 }, 5, 6);
			CheckSorting(new[] { int.MaxValue, 3, 2, 4, 2, 5, int.MinValue }, 5, 6);
			CheckSorting(new[] { int.MaxValue, 3, 2, 4, 2, 5, int.MaxValue }, 5, 6);
			CheckSorting(new[] { int.MinValue, 3, 2, 4, 2, 5, int.MaxValue }, 5, 6);
		}

		[HideOnSlide]
		private static void CheckSorting(int[] arr, int left, int right)
		{
			var arr2 = (int[])arr.Clone();
			BubbleSortRange(arr, left, right);
			try
			{
				for (int i = 0; i < arr.Length; i++)
					if ((i < left || i > right) && arr2[i] != arr[i])
						throw new ApplicationException("Do not change array outside range [left, right]");
				for (int i = left + 1; i <= right; i++)
					if (arr[i - 1] > arr[i])
						throw new ApplicationException("Range [left, right] is not sorted");

			}
			catch (ApplicationException e)
			{
				Console.WriteLine("Sorting range [left={0}, right={1}] in array {2}", left, right, string.Join(" ", arr2.Select(a => a.ToString()).ToArray()));
				Console.WriteLine("Wrong result: {0}", string.Join(" ", arr.Select(a => a.ToString()).ToArray()));
				Console.WriteLine(e.Message);
				Console.WriteLine();
			}
		}
	}
}