using System;

namespace uLearn.tests
{
	[Slide("title", "8abc175fee184226b45b180dc44f7aec")]
	internal class CommentsInsideCodeBlock
	{
		[ShowBodyOnSlide]
		public void DoSample()
		{
			//			Not a block!

			/*
			Not a block too
			*/
			Console.WriteLine("Hello Sample!");
		}

		public class C
		{
			/*
			Not a block again
			*/
		}

		public class C2
		{
			/*
			Not a block never
			*/
			public void X()
			{
			}

			/* 
			Not a block ever
			*/
		}
	}
}