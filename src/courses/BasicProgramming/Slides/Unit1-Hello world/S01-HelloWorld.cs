using System;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Title("Привет, мир!")]
	public class S01_HelloWorld
	{

		/*
		##Задача: Обряд посвящения
		
		Любая, даже самая сложная дорога, начинается с первого шага.

		Выведите на консоль фразу ```The first step!```
		*/

		[Exercise]
		[ExpectedOutput("The first step!")]
		[Hint("Вывод на консоль производится методом Console.WriteLine")]
		static public void Main()
		{
			Console.WriteLine("The first step!");
			/*uncomment
			// пишите код тут
			*/
		}
	}

	
}
