using System;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Title("Строки")]
	class S04_Strings
	{
		/*
		##Задача: Две половинки одного целого
		
		Нужно сделать так, чтобы код писал на экран вторую половину строки text.
		*/
		[Exercise]
		[ExpectedOutput("CSharp!")]
		static public void Main()
		{
			string text = "I love CSharp!";
			string half = text.Substring(text.Length / 2);
			Console.WriteLine(half);
			/*uncomment
			string text = "I love CSharp!";
			string half = ...;
			Console.WriteLine(half);
			*/
		}
	}
}
