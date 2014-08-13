using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.Courses.BasicProgramming.Slides.Slides.U03_Cycles
{
	[Slide("Операторы If и Else", "{BEB02065-75DF-4443-B0DA-DE00971DC36B}")]
	class S020_IfElse
	{
		//#video //www.youtube.com/embed/g0YDCQn0b1E?rel=0

		/*
		## Заметки по лекции
		*/

		public static void MainX()
		{
			var a = int.Parse(Console.ReadLine());

			if (a == 0) Console.WriteLine("A is zero");
			// если действие if состоит из одного оператора, его можно писать без фигурных скобок

			if (a == 1)
			{
				//В противном случае нужно обнести нужные операторы скобками
				Console.Write("Let me think... ");
				Console.WriteLine("A is one!");
			}

			if (a % 2 == 0) Console.WriteLine("A is odd");
			else Console.WriteLine("A is even");

			if (a < 0) Console.WriteLine("A is negative");
			else if (a < 10) Console.WriteLine("A consists of one digit");
			else
			{
				var num = a.ToString().Length;
				Console.WriteLine("The number of digits in A is {0}", num);
				if (a > 1000)
					Console.Write("A is enormous!");
			}
		}
	}
}
