using System;
using uLearn;
using uLearn.CSharp;

namespace Presentation.Slides.U99_Presentation
{
	[Slide("Slide", "{15AE3D7A-C9CB-436C-B2B9-1504D1339208}")]
	class _Attributes : SlideTestBase
	{
		[ExpectedOutput("Hello, world!")]
		public static void Main()
		{
			Console.Write("Hello, world");
			ShowOnSlide();
			HideOnSlide();
			ExcludeFromSolution();
			Exercise();
		}

		[Exercise]
		[Hint("Hint!")]
		private static void Exercise()
		{
			Console.WriteLine('!');
			/*uncomment Without code*/
		}

		[ExcludeFromSolution]
		private static void ExcludeFromSolution()
		{
			return;
		}

		[HideOnSlide]
		private static void HideOnSlide()
		{
			return;
		}

		private static void ShowOnSlide()
		{
			return;
		}
	}
}