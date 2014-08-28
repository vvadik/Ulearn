using System;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Сделай то, не знаю что", "5EE7337B-14FE-41A2-9E12-ACA989534A3A")]
	class S095_Quest
	{
		/*
		Задача-загадка. Попробуйте сами понять, что нужно сделать :-)
		*/

		[ExpectedOutput(@"
Decode(<0>) = 0
Decode(<123>) = 123
Decode(<10-10>) = 1010
Decode(<102-5>) = 1
Decode(<--->) = -1
Decode(<lkj>) = -1
Decode(<65536>) = 0
Decode(<9223372036854775807>) = 1023
Additional secret tests — PASSED
")]

		[HideOnSlide]
		[Hint("Внимательно изучите ошибки, что они значат?")]
		[Hint("Попробуйте понять из текста ошибок какого метода не хватает. Не бойтесь экспериментировать!")]
		[Hint("Остаток от деления берется с помощью оператора `%`")]
		public static void Main()
		{
			Check("<0>");
			Check("<123>");
			Check("<10-10>");
			Check("<102-5>");
			Check("<--->");
			Check("<lkj>");
			Check("<65536>");
			Check("<9223372036854775807>");
			Console.WriteLine("Additional secret tests — {0}", CheckSecretTests() ? "PASSED" : "FAILED");
		}
		
		[HideOnSlide]
		private static bool CheckSecretTests()
		{
			for (long i = 0; i < 2000; i++)
				if (Decode("<-" + i + "->") != i % 1024)
					return false;
			for (long i = 0; i < 2000; i++)
				if (Decode("<-" + (long.MaxValue - i) + "->") != (long.MaxValue - i) % 1024)
					return false;
			if (Decode("<>") != -1) 
				return false;
			return true;
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
				return long.Parse(code.Substring(1, code.Length - 2).Replace("-", "")) % 1024;
			}
			catch
			{
				return -1;
			}
		}
	}
}
