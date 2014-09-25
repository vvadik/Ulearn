using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U03_Cycles
{
	[Slide("Рамочка", "{6923AA65-95E0-4B5E-A98D-B286FD0B2153}")]
	class S055_Border : SlideTestBase
	{
		/*
		Вы решили помочь Васе с разработкой его игры и взяли на себя красивый вывод сообщений в игре.

		Напишите функцию, которая принимает на вход строку текста и печатает ее на экран в рамочке 
		из символов +, - и |. Для красоты текст должен отделяться от рамки слева и справа пробелом.

		Например, текст Hello world должен выводиться так:

		
			+-------------+
			| Hello world |
			+-------------+
		
		*/

		[ExpectedOutput(@"
+-------+
| Menu: |
+-------+
+--+
|  |
+--+
+---+
|   |
+---+
+------------+
| Game Over! |
+------------+
+---------------+
| Select level: |
+---------------+

")]
		public static void Main()
		{
			WriteTextWithBorder("Menu:");
			WriteTextWithBorder("");
			WriteTextWithBorder(" ");
			WriteTextWithBorder("Game Over!");
			WriteTextWithBorder("Select level:");
		}

		[Exercise]
		[Hint("Разбейте код на три части — верх рамки, середина и низ рамки")]
		[Hint("Верх и низ рамки одинаковый, можно один раз сформировать строку и вывести ее дважды — в начале и в конце")]
		private static void WriteTextWithBorder(string text)
		{
			string bar = "";
			for(int i=0; i<text.Length+2; i++)
				bar += "-";
			bar = "+" + bar + "+";
			Console.WriteLine(bar);
			Console.WriteLine("| " + text + " |");
			Console.WriteLine(bar);
		}
	}
}
