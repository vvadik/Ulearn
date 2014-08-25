using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.Courses.BasicProgramming.Slides.U08_Recursion
{
	[Slide("Перестановка", "{198CA6C0-BD3B-4E90-AE96-9503DB0CC4E6}")]
	class S070_Permutations
	{
		//#video d9Beustq6Wg
		/*
		## Заметки по лекции
		*/
		static int[,] prices = new int[,]
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
				bool found = false;
				for (int j = 0; j < position; j++)
					if (permutation[j] == i)
					{
						found = true;
						break;
					}
				if (found) continue;
				permutation[position] = i;
				MakePermutations(permutation, position + 1);
			}
		}

		static void MainX()
		{
			MakePermutations(new int[4], 0);
		}
	}
}
