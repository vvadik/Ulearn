using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U99_Presentation
{
	[Slide("Slide", "{15AE3D7A-C9CB-436C-B2B9-1504D1339208}")]
	class _Attributes
	{
		/*
		как работают Слайды
		*/

		[ExpectedOutput("Hello, world!")]
		public static void Main()
		{
			Console.Write("Hello, world");
			ShowOnSlide();
			HideOnSlide();
			ExcludeFromSolution();
			Exercise();
			ExcludeAndHide();
		}

		[Exercise]
		[Hint("Hint!")]
		private static void Exercise()
		{
			Console.WriteLine('!');
			/*uncomment
			Without code
			*/
		}

		[ExcludeFromSolution]
		private static void ExcludeFromSolution()
		{
			throw new NotImplementedException();
		}

		[HideOnSlide]
		private static void HideOnSlide()
		{
			throw new NotImplementedException();
		}

		private static void ShowOnSlide()
		{
			throw new NotImplementedException();
		}

		[ExcludeFromSolution]
		[HideOnSlide]
		private static void ExcludeAndHide()
		{
			throw new NotImplementedException();
		}
	}
}