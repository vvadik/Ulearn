using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U04_Arrays
{
	/*
	Очень важно уметь работать с массивами, потому что задач, где приходится работать
	с наборами элементов очень много. Предлагаем вам решить одну из них. 
	Следует написать метод ```FindMediana(int[] arr)```, который возвращает [медиану](http://life-prog.ru/view_statistika.php?id=8) массива.
	Сам он принимает одномерный массив целых чисел. Количество чисел в массиве - нечетно..
	*/
	[Slide("Медиана", "{735AD000-BB8B-440B-93F7-37960EEC9800}")]
	class S021_ArrayExercise
	{
		[ExpectedOutput("5\r\n8\r\n444")]
		public static void Main()
		{
			var first = new int[] {5, 2, 8, 9, 3, 1, 90};
			var second = new int[] {4, 8, 9, 1, 3, 5, 77, 8, 10};
			var third = new int[] {777, 666, 444, 111, 333};
			Console.WriteLine(FindMediana(first));
			Console.WriteLine(FindMediana(second));
			Console.WriteLine(FindMediana(third));
		}

		[Exercise]
		private static int FindMediana(int[] arr)
		{
			return arr.OrderBy(x => x).ToArray()[arr.Length/2];
			/*uncomment
			...
			*/
		}
	}
}
