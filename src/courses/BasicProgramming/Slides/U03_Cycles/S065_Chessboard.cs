using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Шахматная доска", "{8E561342-9F4B-411D-BB9B-741A3677F696}")]
	class S065_Chessboard
	{
		/*
		Стали известны подробности про новую игру Васи. Оказывается её действия разворачиваются на шахматных досках нестандартного размера.

		У Васи уже написан код, генерирующий стандартную шахматную доску размера 8х8.
		Помогите Васе переделать этот код так, чтобы он умел выводить доску любого заданного размера.

		Например, доска зармера три должна выводиться так:

			 # 
			# #
			 # 
		*/

		[ExpectedOutput(@"
 # # # #
# # # # 
 # # # #
# # # # 
 # # # #
# # # # 
 # # # #
# # # # 

 

 #
# 

 # 
# #
 # 

 # # # # #
# # # # # 
 # # # # #
# # # # # 
 # # # # #
# # # # # 
 # # # # #
# # # # # 
 # # # # #
# # # # # 
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
			bool firstIsBlack = false;
			for(int y=0; y<size; y++)
			{
				bool isBlack = firstIsBlack;
				for (int x = 0; x < size; x++)
				{
					Console.Write(isBlack ? "#" : " ");
					isBlack = !isBlack;
				}
				firstIsBlack = !firstIsBlack;
				Console.WriteLine();
			}
			Console.WriteLine();

			/*uncomment
			Console.WriteLine(" # # # #");
			Console.WriteLine("# # # # ");
			Console.WriteLine(" # # # #");
			Console.WriteLine("# # # # ");
			Console.WriteLine(" # # # #");
			Console.WriteLine("# # # # ");
			Console.WriteLine(" # # # #");
			Console.WriteLine("# # # # ");
			Console.WriteLine();
			*/
		}
	}
}
