using System;
using System.Globalization;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Заключение", "{C232BC62-FEB7-450F-A0F5-28911F4146A5}")]
	class S099
	{
		/*
			Вооруженные знаниями, вы теперь можете написать красивое и лаконичное решение следующей задачи:
			Вам требуется написать программу для нахождения дискриминанта квадратного уравнения с коэффицентами ```a,b,c```. 
			Вид уравнения ax^2+by+c = 0. Если уравнение имеет нулевой коэффицент, то вывести "None";
		*/

		[ExpectedOutput("-8\r\nNone\r\n16\r\nNone")]
		public static void Main()
		{
			FindDiscriminant(1, 2, 3);
			FindDiscriminant(1, 0, 9);
			FindDiscriminant(1, 2, -3);
			FindDiscriminant(4, 3, 0);
		}

		[Exercise]
		private static void FindDiscriminant(int a, int b, int c)
		{
			try
			{
				int test = 1/a/b/c;
				Console.WriteLine(b*b - 4*a*c);
			}
			catch
			{
				Console.WriteLine("None");
			}
			
			/*uncomment
			...
			*/
		}
	}
}
