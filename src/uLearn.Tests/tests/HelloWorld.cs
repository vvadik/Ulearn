using System;
using uLearn.CSharp;

namespace uLearn.tests
{
	[Slide("title", "id")]
	public class S01_HelloWorld
	{
		/*
		##Задача: Обряд посвящения
		Любая, даже самая сложная дорога, начинается с первого шага, поэтому просто поприветствуйте котика фразой "Hello, kitty!"
		*/

		[ExpectedOutput("Hello, kitty!")]
		public void Main()
		{
			HelloKitty();
		}

		[Exercise]
		static public void HelloKitty()
		{
			Console.WriteLine("Hello, kitty!");
			/*uncomment
			...
			*/
		}
	}
}
