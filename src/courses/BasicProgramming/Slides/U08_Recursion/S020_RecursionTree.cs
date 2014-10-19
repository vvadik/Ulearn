using System;

namespace uLearn.Courses.BasicProgramming.Slides.U08_Recursion
{
	[Slide("Дерево рекурсии", "{2436D8F5-FE23-4071-A198-B1D5B059AE09}")]
	class S020_Tree
	{
		//#video ruWyL5k1tG0
		/*
		##Заметки по лекции
		*/

		public static void Make(int n)
		{
			for (int i = n - 1; i >= 0; i--)
			{
				Console.Write(i + " ");
				Make(i);
			}
		}

		/*
		![карта памяти](_20_map.png)

		Расчет сложности:
		$$
		p(n)=1+\sum_{i=0}^{n-1} p(i) = 2^n
		$$
		*/

	}
}
