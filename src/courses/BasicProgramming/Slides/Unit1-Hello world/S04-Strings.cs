using System;

namespace uLearn.Courses.BasicProgramming.Slides
{
	class S04_Strings
	{
		/*
		##Задача: Две половинки одного целого
		TODO
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
