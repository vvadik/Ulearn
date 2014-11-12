using System;
using NUnit.Framework;
using uLearn;

namespace uLearn.Courses.BasicProgramming.Slides.U11_OOP
{
	[Slide("Методы", "0C32CA2B-7985-467C-9796-1A9DF1369406")]
	public class S030_Methods
	{
		//#video cH67-457qK8
		/*
		## Заметки по лекции
		*/
		class Point
		{
			public int X;
			public int Y;
			public void Print()
			{
				Console.WriteLine("{0} {1}", X, Y);
			}

			public static void PrintPoint(Point point)
			{
				Console.WriteLine("{0} {1}", point.X, point.Y);
			}
		}

		[Test]
		public static void Main()
		{
			var point = new Point { X = 1, Y = 2 };
			point.Print();
			Point.PrintPoint(point);
		}
	}
}