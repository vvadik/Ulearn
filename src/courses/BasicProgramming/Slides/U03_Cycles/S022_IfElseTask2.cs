using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.Slides.U03_Cycles
{
	[Slide("Операторы If и Else 3", "{937C4E64-7144-4F52-A75D-4BDC95BBDE72}")]
	class S022_IfElseTask2
	{
		/*
		А теперь Васе очень хочется находить среднее число из 3х данных чисел. Помогите ему в этом.
		*/

		[ExpectedOutput("False\nFalse\nTrue\nFalse\nFalse\nTrue\nFalse\nTrue")]
		public void Main()
		{
			Console.WriteLine(MiddleOf(2, 3, 4));
			Console.WriteLine(MiddleOf(5, -5, 55));
			Console.WriteLine(MiddleOf(12, 12, 11));
			Console.WriteLine(MiddleOf(2, 3, 2)); 
			Console.WriteLine(MiddleOf(8, 8, 8)); 
			Console.WriteLine(MiddleOf(5, 0, 1));
		}

		[Exercise]
		private int MiddleOf(int num1, int num2, int num3)
		{
			return new[] {num1, num2, num3}.OrderBy(x => x).ToArray()[1];
			/*uncomment
			...
			*/
		}
	}
}
