using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Неверный тип данных", "{38FD7DB3-E4E5-4EC1-ACBD-30F3A55A70F4}")]
	public class S020_TypesExercise
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
")]
		[Hint("5.5e-2 — это так называемая [Экспоненциальная запись](https://ru.wikipedia.org/wiki/%D0%AD%D0%BA%D1%81%D0%BF%D0%BE%D0%BD%D0%B5%D0%BD%D1%86%D0%B8%D0%B0%D0%BB%D1%8C%D0%BD%D0%B0%D1%8F_%D0%B7%D0%B0%D0%BF%D0%B8%D1%81%D1%8C) действительного числа")]
		[Hint("Обратите внимание на суффиксы f и L после чисел. Что они означают?")]
		public static void Main()
		{
			var num1 = +5.5e-2;
			var num2 = 7.8f;
			var num3 = 0;
			var num4 = 2000000000000L;
			Console.WriteLine(num1);
			Console.WriteLine(num2);
			Console.WriteLine(num3);
			Console.WriteLine(num4);
			/*uncomment
			... num1 = +5.5e-2;
			... num2 = 7.8f;
			... num3 = 0;
			... num4 = 2000000000000L;
			Console.WriteLine(num1);
			Console.WriteLine(num2);
			Console.WriteLine(num3);
			Console.WriteLine(num4);
			*/
		}
	}
}