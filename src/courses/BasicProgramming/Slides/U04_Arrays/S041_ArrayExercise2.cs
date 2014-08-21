using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U04_Arrays
{
	/*
	В этом задании вам понадобится четкое понимание типов ссылок и типов значения.
	Есть метод ```FeedSnakeAndReport(int[] snake)```, который "кормит" змею. Что это значит:
	суть змейки - массив [1,2,3,0,0,0] - змейка длины 6, которая сыта на троечку =)
	Разумеется, есть и более длинные/короткие змейки. Что представляет собой кормление?
	Это увеличение ее сытости на один, то есть в данном случае ```FeedSnakeAndReport``` изменит массив
	на [1,2,3,4,0,0],и сытость станет равна 4. Если кормление возможно - кормим, печатаем текущую сытость,
	иначе, если сытость полная, - сообщение "Full".
	Вам следует реализовать метод ```FeedSnakeAndReport```. Удачи!
	*/

	[Slide("Змейки", "{E3E45EC7-7BD0-4284-8CA1-0FBCB2FA0C21}")]
	class S041_ArrayExercise2
	{
		[ExpectedOutput("3\n5\n6\n4\nFull\n6\n7")]
		public static void Main()
		{
			var lusy = new int[] {1, 2, 0, 0, 0, 0};
			var tean = new int[] {1, 2, 3, 4, 0, 0, 0, 0, 0, 0};
			var python = new int[] {1, 2, 3, 4, 5, 0, 0, 0};
			var cobra = new int[] {1, 2, 3, 0};
			FeedSnakeAndReport(lusy);
			FeedSnakeAndReport(tean);
			FeedSnakeAndReport(python);
			FeedSnakeAndReport(cobra);
			FeedSnakeAndReport(cobra);
			FeedSnakeAndReport(tean);
			FeedSnakeAndReport(python);
		}

		[Exercise]
		public static void FeedSnakeAndReport(int[] snake)
		{
			if (IsPossibleToFeed(snake))
			{
				var toFeedIndex = string.Join("", snake).IndexOf('0');
				snake[toFeedIndex] = snake[toFeedIndex - 1] + 1;
				Console.WriteLine(snake[toFeedIndex - 1] + 1);
				return;
			}
			Console.WriteLine("Full");
			/*uncomment
			 ...
			*/
		}

		[HideOnSlide]
		[ExcludeFromSolution]
		public static bool IsPossibleToFeed(int[] snake)
		{
			return snake.Length > snake.Max();
		}
	}
}
