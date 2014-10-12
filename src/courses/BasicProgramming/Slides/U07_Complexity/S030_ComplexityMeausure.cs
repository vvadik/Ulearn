using System;

namespace uLearn.Courses.BasicProgramming.Slides.U07_Complexity
{
	[Slide("Измерение сложности", "{0820F9D6-0B3A-4A91-99B3-ABC4DA5A0521}")]
	class S030_ComplexityMeausure
	{
		//#video 78BFvmgOoNM

		/*
		##Заметки по лекции
		*/

		// ReSharper disable PossibleNullReferenceException
		[ShowBodyOnSlide]
		public void Complexity()
		{
			var n = Console.ReadLine().Length;
			var sum = 0;
			for (int i = 0; i < n; i++)
				for (int j = 0; j < 2 * i; j++)
					sum++;
			Console.WriteLine(sum);
		}
		// ReSharper restore PossibleNullReferenceException

		/*
		Этот код считает следующую сумму $sum=0+2+4+\ldots+2(n-1)=n(n-1)$

		Вычислительная сложность этого алгоритма $f(n)$:

		$$f(n)=n(n-1)(2_{++}+1_{*}+1_{<})+n(1_=+1_{++}+1_<)+2_=+RL+WL=$$
		$$=k_{R}n+k_{W}\lceil\log_{10}n(n-1)\rceil+4n^2-n+2$$
		*/
	}
}
