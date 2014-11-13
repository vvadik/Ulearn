using System;
using System.Linq;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U09_Sorting
{
	[Slide("Рекурсивный бинарный поиск", "4D7960D87D654FF7B07036E8FF618755")]
	public class S045_BinSearch : SlideTestBase
	{
		/*
		В разных условиях нужны разные модификации бинарного поиска. Всего есть четыре модификации:

		* Найти последний элемент меньший заданного (левая граница)
		* Найти первый элемент равный заданному (первое вхождение)
		* Найти последний элемент равный заданному (последнее вхождение)
		* Найти первый элемент больший заданного (правая граница)

		![bin-search](bin-search.png)

		Например, с помощью поиска правой и левой границы можно вычислить количество элементов в массиве, значения которых лежат в заданном диапазоне [minValue, maxValue]:
		
			count = BinSearchRightBorder(arr, maxValue, -1, arr.Length) - BinSearchLeftBorder(arr, minValue, -1, arr.Length) + 1;
		
		Заметим, что задачу поиска первого вхождения легко свести к задаче поиска левой границы. И аналогично последнее вхождение свести к правой границе.

		При поиске границ удобно считать, что левая граница может принимать значение -1, если уже нулевой элемент равен заданному.
		А правая граница может принимать значение `array.Length`, если последний элемент равен заданному.
		Естественно, тогда и начинать бинарный поиск нужно с `left=-1` и `right=array.Length`.
		
		В этой задаче вам нужно дописать рекурсивную версию бинарного поиска, который находит левую границу, то есть индекс _максимального_ элемента _меньшего_ `value`.
		*/

		private static int FindLeftBorder(long[] arr, long value)
		{
			return BinSearchLeftBorder(arr, value, -1, arr.Length);
		}

		[Exercise]
		public static int BinSearchLeftBorder(long[] array, long value, int left, int right)
		{
			if (right - left <= 1)
				return left;
			var m = (left + right) / 2;
			if (array[m] < value)
				return BinSearchLeftBorder(array, value, m, right);
			return BinSearchLeftBorder(array, value, left, m);
			/*uncomment
			if (...) return left;
			var m = (left + right) / 2;
			if (array[m] < value)
				return BinSearchLeftBorder(array, value, ...);
			return BinSearchLeftBorder(array, value, ...);
			*/
		}

		[ExpectedOutput("")]
		[HideOnSlide]
		public static void Main()
		{
			RunTest(new long[] { 1, 2, 3 }, 0, -1);
			RunTest(new long[] { 1, 2, 3 }, 1, -1);
			RunTest(new long[] { 1, 2, 3 }, 2, 0);
			RunTest(new long[] { 1, 2, 3 }, 3, 1);
			RunTest(new long[] { 1, 2, 3 }, 4, 2);
			RunTest(new long[] { }, 1, -1);
			RunTest(new long[] { 1 }, 0, -1);
			RunTest(new long[] { 1 }, 1, -1);
			RunTest(new long[] { 1 }, 2, 0);
			RunTest(new long[] { 1, 1 }, 0, -1);
			RunTest(new long[] { 1, 1 }, 1, -1);
			RunTest(new long[] { 1, 1 }, 2, 1);
			RunTest(new long[] { 1, 2, 2, 3 }, 2, 0);
			RunTest(new long[] { 1, 2, 2, 2, 3 }, 2, 0);
			RunTest(new long[] { 1, 1, 1, 2, 3 }, 2, 2);
		}

		[HideOnSlide]
		private static void RunTest(long[] arr, long value, int expectedIndex)
		{
			var index = FindLeftBorder(arr, value);
			if (expectedIndex != index)
				Console.WriteLine("FindLeftBorder(new long[]{{{0}}}, {1}) gives {2} which is wrong!", string.Join(",", arr.Select(a => a.ToString()).ToArray()), value, index);
		}
	}
}