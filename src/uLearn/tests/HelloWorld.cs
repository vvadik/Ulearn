using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[TestFixture]
	public class S01_HelloWorld
	{
		/*
		##Задача: Обряд посвящения
		Любая, даже самая сложная дорога, начинается с первого шага, поэтому просто поприветствуйте котика фразой "Hello, kitty!"
		*/

		public void Main()
		{
			HelloKitty();
		}

		[Exercise(SingleStatement = true)]
		[ExpectedOutput("Hello, kitty!")]
		static public void HelloKitty()
		{
			Console.WriteLine("Hello, kitty!");
			/*uncomment
			...
			*/
		}

		[Test]
		public void Test()
		{
			TestExerciseStaff.TestExercise(GetType().GetMethod("HelloKitty"));
		}
	}
}
