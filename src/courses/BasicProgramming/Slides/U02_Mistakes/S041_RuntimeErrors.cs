using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Деление на ноль", "{754B8DCB-E6F6-44A9-AEE6-9972BEB9AF7C}")]
	class S041_RuntimeErrors
	{
		/*
		Делить на ноль нельзя! Всегда надо проверять, что вы не делите не ноль!
		А вдруг враги подсунут вам ноль? Тогда все сломается.
		Но написать метод Divide, который производит целочисленное деление первого числа на второе, все-же придется.
		Результат деления нужно выводить на консоль. Если делить нельзя, выводите строку "impossible".
		*/

		[ExpectedOutput("24\r\nimpossible\r\n-30\r\n5\r\n3\r\nimpossible\r\nimpossible\r\n-17\r\n0\r\nimpossible")]
		public static void Main()
		{
			Divide(120, 5);
			Divide(120, 0);
			Divide(120, -4); 
			Divide(120, 22); 
			Divide(120, 35);
			Divide(120, 0);
			Divide(120, 0);
			Divide(120, -7); 
			Divide(120, 1111);
			Divide(120, 0);
		}

		[Exercise]
		private static void Divide(int divident, int divider)
		{
			if (IsBadDivider(divider))
			{
				Console.WriteLine("impossible");
				return;
			}
			var a = divident / divider;
			Console.WriteLine(divident/divider);
			/*uncomment
			...
			*/
		}

		[HideOnSlide]
		private static bool IsBadDivider(int divider)
		{
			return divider == 0;
		}
	}
}
