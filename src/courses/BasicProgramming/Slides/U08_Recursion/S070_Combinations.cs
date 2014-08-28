using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.Courses.BasicProgramming.Slides.U08_Recursion
{
	[Slide("Размещения", "{198CA6C0-BD3B-4E90-AE96-9503DB0CC4E6}")]
	class S070_Combination
	{
		//#video d9Beustq6Wg
		/*
		## Заметки по лекции
		*/
		static void Combinations(bool[] combination, int onesLeft, int rightEdge)
		{
			if (onesLeft == 0)
			{
				foreach (var c in combination.Reverse())
					Console.Write(c ? 1 : 0);
				Console.WriteLine();
				return;
			}

			for (int j = rightEdge; j < combination.Length - onesLeft + 1; j++)
			{
				combination[j] = true;
				Combinations(combination, onesLeft - 1, j + 1);
				combination[j] = false;
			}
		}

		static IEnumerable<bool[]> Combinations1(int N, int K)
		{
			var combination = new bool[N];
			for (int i = N - K; i < N; i++) combination[i] = true;

			while (true)
			{
				yield return combination.ToArray();
				int j = N - 1;
				int ones = 0;
				for (j = N - 1; j >= 1; j--)
				{
					if (combination[j])
					{
						if (!combination[j - 1]) break;
						ones++;
					}
				}
				if (j == 0) yield break;
				combination[j - 1] = true;
				for (int k = j; k < N - ones; k++)
					combination[k] = false;
				for (int k = N - ones; k < N; k++)
					combination[k] = true;

			}
		}


		public static void MainX()
		{
			foreach (var e in Combinations1(5, 3))
			{
				foreach (var c in e)
					Console.Write(c ? 1 : 0);
				Console.WriteLine();
			}

		}
	}
}
