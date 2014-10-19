using System;

namespace uLearn.Courses.BasicProgramming.Slides.U08_Recursion
{
	[Slide("Рекурсия", "{DB1B681F-5283-4AB8-8A78-EC1A202BA478}")]
	class S010_Recursion
	{
		//#video doR1W-hI3qU
		/*
		## Заметки по лекции
		*/

		static void Make(int n)
		{
			for (int i = n - 1; i >= 0; i--)
			{
				Console.Write(i.ToString() + " ");
				Make(i);
			}
		}

		public static void Main()
		{
			Make(4);
			Console.WriteLine();
		}
	}
}
