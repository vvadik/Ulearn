using System;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Title("Типы данных")]
	public class S02_Types
	{
		/*
		##Задача: Соответствие типов данных
		
		Неправильно указанный тип данных - частая ошибка в работе программиста. Вот и сейчас программист Вася в замешательстве. Исправьте положение!
		*/
		[Exercise]
		[ExpectedOutput("5.5\r\n7.8\r\n0")]
		static public void Main()
		{
			var a = 5.5;
			var b = 7.8f;
			var c = 0;
			Console.WriteLine(a);
			Console.WriteLine(b);
			Console.WriteLine(c);
			/*uncomment
			... a = 5.5;
			... b = 7.8f;
			... c = 0;
			Console.WriteLine(a);
			Console.WriteLine(b);
			Console.WriteLine(c);
			*/
		}
	}
}
