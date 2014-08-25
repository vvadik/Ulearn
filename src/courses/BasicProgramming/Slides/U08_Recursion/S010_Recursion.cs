using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			Console.Write("Make(1): ");
			Make(1);
			Console.WriteLine();

			Console.Write("Make(2): ");
			Make(2);
			Console.WriteLine();

			Console.Write("Make(3): ");
			Make(6);
			Console.WriteLine();
		}
	}
}
