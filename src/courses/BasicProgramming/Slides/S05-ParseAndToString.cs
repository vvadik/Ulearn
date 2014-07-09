using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[TestFixture]
	public class S05_ParseAndToString
	{
		/*
		##Задача: Парсинг
		Гейтс написал код и не может найти ошибку :( 
		Помогите ему!
		*/
		[Exercise(SingleStatement = true)]
		[ExpectedOutput("894377.243643")]
		static public void ParseAndToString()
		{
			string doubleNumber = "894376.243643";
			double number = double.Parse(doubleNumber, CultureInfo.InvariantCulture);
			Console.WriteLine(number + 1);
			/*uncomment
			string doubleNumber = "894376.243643";
			int number = doubleNumber;
			Console.WriteLine(number + 1);
			*/
		}

		[Test]
		public void Test()
		{
			TestExerciseStaff.TestExercise(GetType().GetMethod("ParseAndToString"));
		}
	}
}
