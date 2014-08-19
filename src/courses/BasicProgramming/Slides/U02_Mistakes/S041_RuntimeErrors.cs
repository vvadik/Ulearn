using System;
using System.Globalization;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Минимум функции", "{754B8DCB-E6F6-44A9-AEE6-9972BEB9AF7C}")]
	class S041_RuntimeErrors
	{
		/*
		Вам надо реализовать функцию для нахождения минимального значения параболы, заданной уравнением вида ax^2+bx+c=0.
		Функция принимает коэффиценты ```a,b,c``` и, если они корректны, печатает минимальное значение, иначе строку "Impossible".
		Все коэффиценты больше или равны нулю.
		*/

		[ExpectedOutput("-1\r\nImpossible\r\n-0.2\r\n-0.375\r\nImpossible")]
		public static void Main()
		{
			FindMinimum(1, 2, 3);
			FindMinimum(0, 3, 2);
			FindMinimum(5, 2, 1);
			FindMinimum(4, 3, 2);
			FindMinimum(0, 4, 5);
		}

		[Exercise]
		private static void FindMinimum(int a, int b, int c)
		{
			Console.WriteLine(a == 0 ? "Impossible" : (-b / (double)(2 * a)).ToString());
			/*uncomment
			...
			*/
		}
	}
}
