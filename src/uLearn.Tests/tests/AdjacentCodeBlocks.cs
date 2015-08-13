using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.tests
{
	[Slide("title", "id")]
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
