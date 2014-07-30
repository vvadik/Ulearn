using System;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Типы данных", "{38FD7DB3-E4E5-4EC1-ACBD-30F3A55A70F4}")]
	public class S02_Types
	{
		/*
		Неправильно указанный тип данных - частая ошибка в работе начинающего программиста.
		Вот и сейчас программист Вася в замешательстве.

		Исправьте положение — укажите у всех объявленных переменных правильные типы вместо многоточия.
		*/

		[Exercise]
		[ExpectedOutput(
			@"0.055
7.8
0
2000000000000
True
hello
h")]
		public static void Main()
		{
			var num1 = +5.5e-2;
			var num2 = 7.8f;
			var num3 = 0;
			var num4 = 2000000000000L;
			var b = true;
			var s = "hello";
			var c = 'h';
			Console.WriteLine(num1);
			Console.WriteLine(num2);
			Console.WriteLine(num3);
			Console.WriteLine(num4);
			Console.WriteLine(b);
			Console.WriteLine(s);
			Console.WriteLine(c);
			/*uncomment
			... num1 = +5.5e-2;
			... num2 = 7.8f;
			... num3 = 0;
			... num4 = 2000000000000L;
			... b = true;
			... s = "hello";
			... c = 'h';
			Console.WriteLine(num1);
			Console.WriteLine(num2);
			Console.WriteLine(num3);
			Console.WriteLine(num4);
			Console.WriteLine(b);
			Console.WriteLine(s);
			Console.WriteLine(c);
			*/
		}
	}
}