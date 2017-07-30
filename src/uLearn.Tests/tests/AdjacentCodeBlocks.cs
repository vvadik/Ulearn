using System;

namespace uLearn.tests
{
	[Slide("title", "8abc175fee184226b45b180dc44f7aec")]
	class AdjacentCodeBlocks
	{
		public void S1()
		{
			Console.WriteLine("S1");
		}

		public void S2()
		{
			Console.WriteLine("S2");
		}

		/* 
		text 
		*/

		[ShowBodyOnSlide]
		public void S3()
		{
			Console.WriteLine("S3");
		}
	}
}