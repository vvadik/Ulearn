using System;

namespace uLearn.Courses.BasicProgramming.Slides.U00_Basics
{
	[Slide("Главный вопрос Вселенной", "{D5B59540-410F-4F6D-8F04-B5613E264BD5}")]
	public class S072_Methods : SlideTestBase
	{

		/*
		Многие знают, 
		что [ответ на главный вопрос жизни, вселенной и всего такого](http://ru.wikipedia.org/wiki/%D0%9E%D1%82%D0%B2%D0%B5%D1%82_%D0%BD%D0%B0_%D0%B3%D0%BB%D0%B0%D0%B2%D0%BD%D1%8B%D0%B9_%D0%B2%D0%BE%D0%BF%D1%80%D0%BE%D1%81_%D0%B6%D0%B8%D0%B7%D0%BD%D0%B8,_%D0%B2%D1%81%D0%B5%D0%BB%D0%B5%D0%BD%D0%BD%D0%BE%D0%B9_%D0%B8_%D0%B2%D1%81%D0%B5%D0%B3%D0%BE_%D1%82%D0%B0%D0%BA%D0%BE%D0%B3%D0%BE)
		— 42. 
		Но Вася хочет большего! Он желает знать квадрат этого числа!

		Вы разделили с Васей работу — он написал главную функцию, а вам осталось лишь дописать методы ```Print``` и ```GetSquare```.
		*/

		[ExpectedOutput("1764")]
		[Hint("Посмотрите лекцию по методам еще раз!")]
		static public void Main()
		{
			Print(GetSquare(42));
		}

		[HideOnSlide]
		[ExcludeFromSolution]
		static private int GetSquare(int i)
		{
			return i * i;
		}

		[HideOnSlide]
		[ExcludeFromSolution]
		static private void Print(int number)
		{
			Console.WriteLine(number);
		}
	}
}
