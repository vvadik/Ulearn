using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.tests
{
	[Slide("title", "id")]
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
			public void X() { }
			/* 
			Not a block ever
			*/

		}
	}
}