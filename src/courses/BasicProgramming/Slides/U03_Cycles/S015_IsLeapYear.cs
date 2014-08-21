using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Високосный год", "{4C161B1E-2637-447B-ADFD-14647BF659AD}")]
	class S015_IsLeapYear
	{
		/*
		Васе очень хочется знать, високосный ли какой-либо год. 
		В [интернете](https://ru.wikipedia.org/wiki/Григорианский_календарь) он нашел способ определения високосности года.
		год является високосным в двух случаях: либо он кратен 4, но при этом не кратен 100, либо кратен 400.
		
		Помогите ему написать программу, которая по введенному году определяет - високосный год или нет.

		Напишите решение в однин оператор.
		*/

		[ExpectedOutput("False\nFalse\nTrue\nFalse\nFalse\nTrue\nFalse\nTrue")]
		public static void Main()
		{
			Console.WriteLine(IsThisLeapYear(2014));
			Console.WriteLine(IsThisLeapYear(1999));
			Console.WriteLine(IsThisLeapYear(8992));
			Console.WriteLine(IsThisLeapYear(1));
			Console.WriteLine(IsThisLeapYear(14));
			Console.WriteLine(IsThisLeapYear(400));
			Console.WriteLine(IsThisLeapYear(600));
			Console.WriteLine(IsThisLeapYear(3200));
		}

		[Exercise]
		[SingleStatementMethod]
		public static bool IsThisLeapYear(int year)
		{
			return year % 4 == 0 && year % 100 != 0 || year % 400 == 0;
			/*uncomment
			return ...
			*/
		}
	}
}
