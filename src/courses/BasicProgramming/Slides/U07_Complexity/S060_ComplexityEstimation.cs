using System;

namespace uLearn.Courses.BasicProgramming.Slides.U07_Complexity
{
	[Slide("Оценка сложности", "{F1991483-E9EC-4367-9ABF-3F773AF5F2C9}")]
	class S060_ComplexityEstimation
	{
		//#video qwx_pCfSyN0
		
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

		/*tex
		f(n)=4n^2+k_1n+k_2\log n(n-1)+2
		g(n)=n^2
		\lim_{n\rightarrow\infty}\frac{4n^2+k_1n+k_2\log n(n-1)+2}{n^2}=
		\lim_{n\rightarrow\infty}\frac{4n^2}{n^2}+\lim_{n\rightarrow\infty}\frac{k_1n}{n^2}+\lim_{n\rightarrow\infty}\frac{k_2 \log n(n-1)}{n^2}+\lim_{n\rightarrow\infty}\frac{2}{n^2}=4
		f(n)=\Theta(g(n))=\Theta\left(n^2\right)
		*/
	}
}
