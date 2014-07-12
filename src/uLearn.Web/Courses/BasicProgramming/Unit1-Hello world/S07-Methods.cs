using System;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Title("Методы")]
	public class S07_Methods
	{
		
		/*

		##Задача: Методы
		
		Как многие из вас знают, ответ на самый главный вопрос человечества - 42. Но, что будет, если это знание возвести в квадрат?
		Узнайте его, реализуя методы Print и GetSquare.
		*/

		[ExpectedOutput("1764")]
		[ShowOnSlide]
		static public void Main()
		{
			Print(GetSquare(42));
		}

		static private int GetSquare(int i)
		{
			return i * i;
		}

		static private void Print(int number)
		{
			Console.WriteLine(number);
		}
	}
}
