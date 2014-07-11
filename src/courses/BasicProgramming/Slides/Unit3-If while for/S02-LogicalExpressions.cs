using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using uLearn;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Title("Логические выражения")]
	class S02_LogicalExpressions
	{
		/*
		##Задача: Логические выражения.
		Вася хочет купить компьютер. Он составил список параметров для всех машин в магазине, устраивает его или нет: цвет, мощность, цена, размер и наличие котиков на процессоре.
		Он знает, что купит компьютер, если выполняется хотябы одно из условий:
		 * его устраивают цвет, мощность и размер
		 * его устраивают цена и мощность
		 * на компьютере есть котики!
		Помогите ему написать программу, которая поможет ему определиться с тем, какие компьютеры подходят под его условия.
		*/
		
		[ShowOnSlide]
		[ExpectedOutput("False\r\nTrue\r\nTrue")]
		public static void Main()
		{
			Console.WriteLine(IsCanBye(true, false, true, false, false));
			Console.WriteLine(IsCanBye(true, true, false, true, false));
			Console.WriteLine(IsCanBye(false, false, false, false, true));
		}

		[Exercise]
		public static bool IsCanBye(bool color, bool power, bool price, bool size, bool kittyOnComputerCase)
		{
			return kittyOnComputerCase || (color && power && size) || (price && power);
			/*uncomment
			return ...
			*/
		}
	}
}
