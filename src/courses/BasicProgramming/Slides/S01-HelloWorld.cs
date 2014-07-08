using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace uLearn.Courses.BasicProgramming.Slides
{
	public class S01_HelloWorld
	{
		/*
        ##Задача: Поприветствуйте котика фразой "Hello, kitty!" :).
        */
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
