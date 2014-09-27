using System;
using uLearn;
using uLearn.CSharp;

namespace OOP.Slides.U01_DataTypes_And_Algorithms
{
	[Slide("Vector.Normalize()", "{863A642D-45C8-4406-91FC-6EC62C243E01}")]
	public class Vector
	{
		/*
		В этой задаче вы можете закрепить свое понимание идеи неизменяемого класса.

		Попробуйте реализовать у класса Vector метод нормирования Normalize так, чтобы класс остался неизменяемым.

		Для этого Normalize не должен менять исходный вектор, а должен вернуть новый вектор, с тем же направлением, но единичной длины.

		Это код, которым ваш вектор будет тестироваться:
		*/

		[Exercise]
		public Vector Normalize()
		{
			var len = Math.Sqrt(X * X + Y * Y);
			return new Vector(X / len, Y / len);
		}

		[ExpectedOutput(@"(1, 0)
(0, -1)
(0.707106781186547, 0.707106781186547)
(-0.98058067569092, 0.196116135138184)
(-0.894427190999916, -0.447213595499958)
(1, 0)
(100, 0)")]
		public static void Main()
		{
			Console.WriteLine(new Vector(1, 0).Normalize());
			Console.WriteLine(new Vector(0, -1).Normalize());
			Console.WriteLine(new Vector(1, 1).Normalize());
			Console.WriteLine(new Vector(-10, 2).Normalize());
			Console.WriteLine(new Vector(-24, -12).Normalize());
			var vector = new Vector(100, 0);
			Console.WriteLine(vector.Normalize());
			Console.WriteLine(vector);
		}

		[HideOnSlide]
		public Vector(double x, double y)
		{
			X = x;
			Y = y;
		}

		[HideOnSlide]
		public double X { get; private set; }

		[HideOnSlide]
		public double Y { get; private set; }

		[HideOnSlide]
		public override string ToString()
		{
			return string.Format("({0}, {1})", X, Y);
		}
	}
}