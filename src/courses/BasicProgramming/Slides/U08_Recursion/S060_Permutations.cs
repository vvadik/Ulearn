using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace uLearn.Courses.BasicProgramming.Slides.U08_Recursion
{
	[Slide("Перестановки", "{DE51CEB3-761A-4308-B042-6AC1C0625956}")]
	class S060_Permutations
	{
		//#video gNYKKnmqoX4
		/*
		## Заметки по лекции
		*/
		static int[,] prices =
		{
			{ 0, 2, 4, 7 },
			{ 2, 0, 3, 1 },
			{ 4, 2, 0, 1 },
			{ 3, 5, 2, 0 }
		};

		static void Evaluate(int[] permutation)
		{
			int price = 0;
			for (int i = 0; i < permutation.Length; i++)
				price += prices[permutation[i], permutation[(i + 1) % permutation.Length]];
			foreach (var e in permutation)
				Console.Write(e + " ");
			Console.Write(price);
			Console.WriteLine();
		}

		static void MakePermutations(int[] permutation, int position)
		{
			if (position == permutation.Length)
			{
				Evaluate(permutation);
				return;
			}

			for (int i = 0; i < permutation.Length; i++)
			{
				var index = Array.IndexOf(permutation, i, 0, position);
				if (index != -1)
					continue;
				permutation[position] = i;
				MakePermutations(permutation, position + 1);
			}
		}

		[Test]
		public static void Main()
		{
			MakePermutations(new int[4], 0);
		}
	}
}
