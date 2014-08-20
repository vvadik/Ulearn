using System;
using System.Globalization;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Сделай то, не знаю что", "5EE7337B-14FE-41A2-9E12-ACA989534A3A")]
	class S099_Quest
	{
		/*
		Задача-загадка. Попробуйте сами понять, что нужно сделать :-)
		*/

		[ExpectedOutput(@"
Decode(<0>) = 0
Decode(<123>) = 123
Decode(<10-10>) = 1010
Decode(<102-5>) = 1
Decode(111) = 1
Decode(1-1-1) = 1
Decode(<--->) = -1
Decode(<lkj>) = -1
Decode(<65536>) = 0
Decode(<9223372036854775807>) = 1023
")]
		
		[HideOnSlide]
		public static void Main()
		{
			Check("<0>");
			Check("<123>");
			Check("<10-10>");
			Check("<102-5>");
			Check("111");
			Check("1-1-1");
			Check("<--->");
			Check("<lkj>");
			Check("<65536>");
			Check("<9223372036854775807>");
		}

		[HideOnSlide]
		private static void Check(string code)
		{
			long res = Decode(code);
			Console.WriteLine("Decode({0}) = {1}", code, res);
		}

		[HideOnSlide]
		[ExcludeFromSolution]
		private static long Decode(string code)
		{
			try
			{
				return long.Parse(code.Substring(1, code.Length-2).Replace("-", "")) % 1024;
			}
			catch
			{
				return -1;
			}
		}
	}
}
