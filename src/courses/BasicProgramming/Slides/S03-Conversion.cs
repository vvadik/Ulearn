using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace uLearn.Courses.BasicProgramming.Slides
{
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

		[Exercise(SingleStatement = true)]
		[ExpectedOutput("14\r\n5")]
		public static void ConvertNumbers()
		{
			double first = 13.59;
			double second = 5.20;
			long third = 99999999999999999;
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

		[Test]
		public void Test()
		{
			TestExerciseStaff.TestExercise(GetType().GetMethod("ConvertNumbers"));
		}
	}
}
