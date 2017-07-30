using System;

namespace uLearn.tests
{
	[Slide("title", "8abc175fee184226b45b180dc44f7aec")]
	class SlideWithComments
	{
		/*
		Comment
		*/

		[ShowBodyOnSlide]
		public void DoSample()
		{
			Console.WriteLine("Hello Sample!");
		}

		/*
		Final
		*/
	}
}