using System;
using uLearn;

namespace Presentation.Slides.U99_Presentation
{
	[Slide("Slide", "{15AE3D7A-C9CB-436C-B2B9-1504D1339208}")]
	public class _Test : SlideTestBase
	{
		[ExpectedOutput("www")]
		public static void Main()
		{
			Console.Write("www");
		}
	}
}