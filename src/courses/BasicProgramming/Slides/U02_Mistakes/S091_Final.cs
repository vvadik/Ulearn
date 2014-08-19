using System;
using System.Globalization;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Заключение", "{C232BC62-FEB7-450F-A0F5-28911F4146A5}")]
	class S097
	{
		/*
			Вооруженные знаниями, вы теперь можете написать красивое и лаконичное решение следующей задачи:
			Вам требуется написать программу для необычного решения квадратного уравнения, которое задано по трем коэффицентам ```a,b,c```. 
			Вид уравнения ax^2+by+c = 0.
			печатает следующие данные:
			-Есть корни - вывести их сумму
			-Нет корней - вывести None
			-Один корень - вывести его.
			Вспомните все, что видели в предыдущих лекциях.
		*/

		[ExpectedOutput("None\r\n-3\r\n-2\r\nNone")]
		public static void Main()
		{
			SolveEq(1, 2, 3);
			SolveEq(1, 6, 9);
			SolveEq(1, 2, -3);
			SolveEq(4, 3, 2);
		}

		[Exercise]
		private static void SolveEq(double a, double b, double c)
		{
			var d = b * b - 4 * a * c;
			
			/*uncomment
			это просто тест, содержание бует другим!
			*/
		}
	}
}
