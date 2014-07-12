using System;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Title("Привет, мир!")]
	public class S01_HelloWorld
	{
	
		/*
		##Задача: Обряд посвящения
		
		Любая, даже самая сложная дорога, начинается с первого шага, поэтому просто поприветствуйте котика фразой "Hello, kitty!"
		*/

		[Exercise]
		[ExpectedOutput("Hello, kitty!")]
		static public void Main()
		{
			Console.WriteLine("Hello, kitty!");
			/*uncomment
			...
			*/
		}
	}

	
}
