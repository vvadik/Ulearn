using System;
using System.Globalization;


namespace uLearn.Courses.BasicProgramming.Slides
{
	[Title("Парсинг")]
	public class S05_ParseAndToString
	{
		/*
		##Задача: Парсинг
		Гейтс написал код и не может найти ошибку :( 
		Помогите ему!
		*/

		[Exercise]
		[ExpectedOutput("894377.243643")]
		public static void Main()
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
	}
}