using System;
using uLearn;
using uLearn.CSharp;

namespace SeleniumCourse.U98_Sample
{
	[Slide("Vector.Normalize()", "{69FDA740-BE13-48A4-83CE-5D515D778A59}")]
	public class S020_ExerciseSlidePage
	{
		/*
		Выведите строку "Hello, World!"
		*/

		[Exercise]
		public static void Print()
		{
			Console.WriteLine("Hello, World!");
		}

		[ExpectedOutput(@"Hello, World!")]
		public static void Main()
		{
			Print();
		}
	}
}