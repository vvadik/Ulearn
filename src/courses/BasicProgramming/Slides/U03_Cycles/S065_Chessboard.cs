using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U03_Cycles
{
	[Slide("Шахматная доска", "{8E561342-9F4B-411D-BB9B-741A3677F696}")]
	class S065_Chessboard : SlideTestBase
	{
		/*
		Стали известны подробности про новую игру Васи. Оказывается ее действия разворачиваются на шахматных досках нестандартного размера.

		У Васи уже написан код, генерирующий стандартную шахматную доску размера 8х8.
		Помогите Васе переделать этот код так, чтобы он умел выводить доску любого заданного размера.

		Например, доска размера пять должна выводиться так:

			██  ██  ██
			  ██  ██  
			██  ██  ██
			  ██  ██  
			██  ██  ██

		Белая клетка обозначается двумя пробелами, черная — двумя символами [black square](http://unicode-table.com/en/25A0/). Вы можете копировать их из примера выше и вставлять в свой код.
		*/

		[ExpectedOutput(@"
██  ██  ██  ██  
  ██  ██  ██  ██
██  ██  ██  ██  
  ██  ██  ██  ██
██  ██  ██  ██  
  ██  ██  ██  ██
██  ██  ██  ██  
  ██  ██  ██  ██

██

██  
  ██

██  ██
  ██  
██  ██

██  ██  ██  ██  ██  
  ██  ██  ██  ██  ██
██  ██  ██  ██  ██  
  ██  ██  ██  ██  ██
██  ██  ██  ██  ██  
  ██  ██  ██  ██  ██
██  ██  ██  ██  ██  
  ██  ██  ██  ██  ██
██  ██  ██  ██  ██  
  ██  ██  ██  ██  ██

")]
		public static void Main()
		{
			WriteBoard(8);
			WriteBoard(1);
			WriteBoard(2);
			WriteBoard(3);
			WriteBoard(10);
		}

		[Exercise]
		private static void WriteBoard(int size)
		{
			for (int y = 0; y < size; y++)
			{
				for (int x = 0; x < size; x++)
					if ((x + y) % 2 == 0) Console.Write("██");
					else Console.Write("  ");
				Console.WriteLine();
			}
			Console.WriteLine();

			/*uncomment
			Console.WriteLine("██  ██  ██  ██  ");
			Console.WriteLine("  ██  ██  ██  ██");
			Console.WriteLine("██  ██  ██  ██  ");
			Console.WriteLine("  ██  ██  ██  ██");
			Console.WriteLine("██  ██  ██  ██  ");
			Console.WriteLine("  ██  ██  ██  ██");
			Console.WriteLine("██  ██  ██  ██  ");
			Console.WriteLine("  ██  ██  ██  ██");
			Console.WriteLine();
			*/
		}
	}
}
