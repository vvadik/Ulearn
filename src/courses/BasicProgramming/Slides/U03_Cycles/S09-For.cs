using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("For", "{67110A78-3932-4810-AF9F-F996799C3110}")]
	class S09_For
	{
		/*
		##Задача: For
		
		Вам в функцию передается число. Необходимо перебрать все числа от 1 до этого числа,
		и для каждого из них вывести его произведение со всеми числами от него самого до еденицы, по-очереди.
		Через пробел и именно в этом порядке.
		*/

		[ExpectedOutput("1 0 4 2 0 9 6 3 0 \r\n1 0 4 2 0 9 6 3 0 16 12 8 4 0")]
		public static void Main()
		{
			for (int i = 3; i < 5; i++)
			{
				Function(i);
				Console.WriteLine();
			}
		}

		[Exercise]
		private static void Function(int number)
		{
			for (int i = 1; i <= number; i++)
			{
				for (int j = i; j >= 0; j--)
				{
					Console.Write(i * j + " ");
				}
			}
			/*uncomment
			...
			*/
		}
	}
}
