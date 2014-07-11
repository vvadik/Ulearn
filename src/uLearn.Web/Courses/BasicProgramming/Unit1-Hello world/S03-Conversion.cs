using System;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Title("Конверсия типов")]
	public class S03_Conversion
	{
		/*
		##Задача: Конверсия типов
		Один тип данных может быть конвертирован в другой, если не произойдет потери данных.
		Выведите на экран в отдельных строках:

		 * первое число - округлённое по математическим правилам,
		 * второе число - приведенное к целочисленному типу,
		 * очистите код от опасных действий
		*/

		[Exercise]
		[ExpectedOutput("14\r\n5")]
		public static void Main()
		{
			double first = 13.59;
			double second = 5.20;
//			long third = 99999999999999999;
			Console.WriteLine(Math.Round(first));
			Console.WriteLine((int) second);
			/*uncomment
			checked
			{
				double first = 13.59;
				double second = 5.20;
				long third = 99999999999999999;
				int integerNumber = (int)third;
			}
			*/
		}
	}
}
