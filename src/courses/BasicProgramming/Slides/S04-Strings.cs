using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace uLearn.Courses.BasicProgramming.Slides
{
	class S04_Strings
	{
		/*
		##Задача: Две половинки одного целого
		Реализуйте метод, который возвращает вторую половину слова, считая, что слово имеет четное количество букв.
		*/

		[Exercise(SingleStatement = true)]
		[ExpectedOutput("CSharp!")]
		static public void MainX()
		{
			PrintHalfWord("I love CSharp!");
			/*uncomment
			 PrintHalfWord("I love CSharp!");
			*/
		}

		private static void PrintHalfWord(string str)
		{
			Console.WriteLine(str.Substring(str.Length/2));
		}

		[Test]
		public void Test()
		{

			TestExerciseStaff.TestExercise(GetType().GetMethod("MainX"));
		}
	}
}
