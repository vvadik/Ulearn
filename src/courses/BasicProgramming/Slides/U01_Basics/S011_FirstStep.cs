using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Первый шаг", "{90BCB61E-57F0-4BAA-8BC9-10C9CFD27F58}")]
	public class S011_FirstStep
	{

		/*
		Любая, даже самая сложная дорога, начинается с первого шага.

		Выведите на консоль фразу `The first step!`

		Допишите код функции `Main`, а затем запустите и проверьте ваше решение с помощью кнопки RUN.
		*/

		[Exercise]
		[ExpectedOutput("The first step!")]
		[Hint("Вывод на консоль производится методом ```Console.WriteLine```")]
		static public void Main()
		{
			Console.WriteLine("The first step!");
			/*uncomment
			// пишите код тут
			*/
		}
	}

	
}
