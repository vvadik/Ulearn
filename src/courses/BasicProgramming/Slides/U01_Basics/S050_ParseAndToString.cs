using System;
using System.Globalization;
using uLearn.CSharp;


namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Преобразование строки в число", "{ACE76228-B605-43B8-AC8D-4C85033AC7DF}")]
	public class S050_ParseAndToString
	{
		/*
		Вася написал код, прибавляющий к числу единичку, но он опять не работает :( 
		
		Немедленно помогите Васе, иначе он решит, что программирование слишком сложно для него!
		*/

		[Exercise]
		[ExpectedOutput("894377.243643")]
		[Hint("Вспомните, как работает операция ```+``` со строковыми аргументами.")]
		[Hint("Функцию преобразования строки в double логично искать среди статических методов класса double")]
		public static void Main()
		{
			string doubleNumber = "894376.243643";
			double number = double.Parse(doubleNumber, CultureInfo.InvariantCulture);
			Console.WriteLine(number + 1);
			/*uncomment
			string doubleNumber = "894376.243643";
			int number = doubleNumber; // Вася уверен, что ошибка где-то тут
			Console.WriteLine(number + 1);
			*/
		}
	}
}