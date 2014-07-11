using System;

namespace uLearn.Courses.BasicProgramming.Slides
{
	public class S07_Methods
	{
		
		/*

		##Задача: Методы
		Как многие из вас знают - ответ на самый главный вопрос человечества - 42. Но, что будет, если это знание возвести в квадрат?
		Узнайте его, реализуя методы Print, GetSquare. А затем узнайте, что было в этот год и поймите истинную природу божественности 42.
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
