using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("В поисках степени двойки", "{4158C9A6-A71D-4015-A283-DEF12BE0055E}")]
	class S041_PowerOfTwo
	{
		/*
		В алгоритмах иногда требуется найти минимальную целую неотрицательную степень двойки превосходящую данное число.

		Решите эту задачу с помощью цикла while.
		Если искомая степень двойки слишком велика и не помещается в тип long, возвращайте -1.
		*/

		
		[Hint(@"Когда переменная целого типа переполняется, она становится отрицательной.
		      Это обусловлено способом хранения числа в двоичном представлении")]
		[ExpectedOutput(@"
4
16
1
1
128
2147483648
2147483648
-1
-1
-1
")]
		public static void Main()
		{
			Console.WriteLine(GetMinPowerOfTwoLargerThan(2)); // => 4
			Console.WriteLine(GetMinPowerOfTwoLargerThan(15)); // => 16
			Console.WriteLine(GetMinPowerOfTwoLargerThan(-2)); // => 1
			Console.WriteLine(GetMinPowerOfTwoLargerThan(-100));
			Console.WriteLine(GetMinPowerOfTwoLargerThan(100));
			Console.WriteLine(GetMinPowerOfTwoLargerThan(2000000000));
			Console.WriteLine(GetMinPowerOfTwoLargerThan(int.MaxValue));
			Console.WriteLine(GetMinPowerOfTwoLargerThan(5000000000000000000));
			Console.WriteLine(GetMinPowerOfTwoLargerThan(9000000000000000000));
			Console.WriteLine(GetMinPowerOfTwoLargerThan(long.MaxValue));
		}

		[Exercise]
		private static long GetMinPowerOfTwoLargerThan(long number)
		{
			unchecked
			{
				long twoPowered = 1;
				while (twoPowered <= number && twoPowered > 0)
					twoPowered *= 2;
				if (twoPowered > number)
					return twoPowered;
				return -1;
			}
			/*uncomment
			...
			*/
		}
	}
}
