using System;

namespace uLearn.Courses.BasicProgramming.Slides.U07_Complexity
{
	[Slide("Сложность алгоритмов с числами", "{0FC6D8B9-8E91-4A9B-A649-2C7E1B3A44A4}")]
	class S070_NumberComplex
	{
		//#video fy4fSRDjPTs
		
		/*
		##Заметки по лекции
		*/

		// ReSharper disable AssignNullToNotNullAttribute
		[ShowBodyOnSlide]
		public void Complexity()
		{
			var n = int.Parse(Console.ReadLine());
			var root = (int)Math.Sqrt(n);
			for (int i = 2; i <= root; i++)
				if (n % i == 0)
				{
					Console.WriteLine(i);
					return;
				}
			Console.WriteLine("N is prime");
		}
		// ReSharper restore AssignNullToNotNullAttribute

		/*tex
		f(n)=\Theta\left(\sqrt{n}\right)
		n=\Theta\left(10^{|x|}\right)
		f(|x|)=\Theta\left(\sqrt{10^{|x|}}\right)
		*/
	}
}
