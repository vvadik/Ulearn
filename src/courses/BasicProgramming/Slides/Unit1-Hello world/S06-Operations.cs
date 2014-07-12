using System;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Title("Арифметические операции и var")]
	public class S06_Operations
	{
		/*
		##Задача: Операции с числами и var
		
		Все, что на первый взгляд кажется очевидным, зачастую оказывается не тем и действует не так, как вы думали.
		Исправьте программу так, чтобы она давала корректный ответ. Объясните, почему программа не работает.
		*/

		[Exercise]
		[Hint("var is very dangerous...")]
		[ExpectedOutput("5.5")]
		static public void Main()
		{
			double a = 5;
			a += 0.5;
			Console.WriteLine(a);
			/*uncomment
			var a = 5;
			a += 0.5;
			Console.WriteLine(a)
			*/
		}


	}
}
