using System;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Логические выражения", "{4C161B1E-2637-447B-ADFD-14647BF659AD}")]
	class S02_LogicalExpressions
	{
		/*
		##Задача: Логические выражения
		
		Вася хочет купить компьютер. Он составил список параметров для всех машин в магазине, устраивает его или нет: цвет, мощность, цена, размер и производитель.
		Он знает, что купит компьютер, если выполняется хотябы одно из условий:

		 * его устраивают цвет, мощность и размер
		 * его устраивают цена и мощность
		 * его устраивает производитель
		
		Помогите ему написать программу, которая поможет ему определиться с тем, какие компьютеры подходят под его условия.
		*/
		
		[ExpectedOutput("False\r\nTrue\r\nTrue")]
		public static void Main()
		{
			Console.WriteLine(CanIBuy(true, false, true, false, false));
			Console.WriteLine(CanIBuy(true, true, false, true, false));
			Console.WriteLine(CanIBuy(false, false, false, false, true));
		}

		[Exercise]
		public static bool CanIBuy(bool color, bool power, bool price, bool size, bool manufacturer)
		{
			return manufacturer || (color && power && size) || (price && power);
			/*uncomment
			return ...
			*/
		}
	}
}
