using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.Courses.BasicProgramming.Slides
{
	public class S01_HelloWorld
	{
		/*
		##Задача: Обряд посвящения
		Любая, даже самая сложная дорога, начинается с первого шага, поэтому просто поприветствуйте котика фразой "Hello, kitty!"
		*/

		[ExpectedOutput("Hello, kitty!")]
		[ShowOnSlide]
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
