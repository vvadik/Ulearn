using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Минимум функции", "{754B8DCB-E6F6-44A9-AEE6-9972BEB9AF7C}")]
	class S041_RuntimeErrors
	{
		/*
		Реализуйте функцию для нахождения такого __x__, при котором парабола ax<sup>2</sup> + bx + c = 0 принимает минимальное значение.
		
		Функция должна принимать неотрицательные коэффиценты __a__, __b__, __c__ и, если решение существует, печатать на консоль искомый __x__ , а иначе — строку `Impossible`.
		*/

		[ExpectedOutput("-1\r\nImpossible\r\n-0.2\r\n-0.375\r\nImpossible\r\nImpossible")]
		[Hint("Помните, что в вашу метод могут передать некорректные значения")]
		[Hint("Используйте `if`")]
		[Hint("Можете погуглить про так называемый 'тернарный оператор' — он позволяет обойтись без `if`")]
		public static void Main()
		{
			WriteParabolaMinX(1, 2, 3);
			WriteParabolaMinX(0, 3, 2);
			WriteParabolaMinX(5, 2, 1);
			WriteParabolaMinX(4, 3, 2);
			WriteParabolaMinX(0, 4, 5);
		}

		[Exercise]
		private static void WriteParabolaMinX(int a, int b, int c)
		{
			Console.WriteLine(a == 0 ? "Impossible" : (-b / (double)(2 * a)).ToString());
			/*uncomment
			...
			*/
		}
	}
}
