using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace uLearn.Courses.BasicProgramming.Slides
{
	public class S05_ParseAndToString
	{
		/*
		##Задача: Гейтс написал код и не может найти ошибку :(, помогите ему.
		*/
		[Exercise(SingleStatement = true)]
		[ExpectedOutput("894376.243643")]
		static public void ParseAndToString()
		{
			string doubleNumber = "894376.243643";
			double number = double.Parse(doubleNumber, CultureInfo.InvariantCulture);
			Console.WriteLine(number);
			/*uncomment
			string doubleNumber = "894376.243643";
			int number = doubleNumber;
			Console.WriteLine(number);
			*/
		}

		[Test]
		public void Test()
		{
			TestExerciseStaff.TestExercise(GetType().GetMethod("ParseAndToString"));
		}
	}
}
