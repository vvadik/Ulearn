using System;
using NUnit.Framework;

namespace uLearn.Courses.BasicProgramming.Slides.U08_Recursion
{
	[Slide("Размещения", "{198CA6C0-BD3B-4E90-AE96-9503DB0CC4E6}")]
	class S070_Combination
	{
		//#video d9Beustq6Wg
		/*
		## Заметки по лекции
		*/
		static void MakeCombinations(bool[] combination, int elementsLeft, int position)
		{
			if (elementsLeft == 0)
			{
				foreach (var c in combination)
					Console.Write(c ? 1 : 0);
				Console.WriteLine();
				return;
			}
			if (position == combination.Length)
				return;

			combination[position] = true;
			MakeCombinations(combination, elementsLeft - 1, position + 1);
			combination[position] = false;
			MakeCombinations(combination, elementsLeft, position + 1);
		}

		[Test]
		public static void Main()
		{
			MakeCombinations(new bool[5], 3, 0);
		}
	}
}
