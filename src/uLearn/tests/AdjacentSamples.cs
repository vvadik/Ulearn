using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.tests
{
	[Slide("title", "id")]
	class AdjacentSamples
	{
		public void Sample1()
		{
			Console.WriteLine("S1");
		}
		public void Sample2()
		{
			Console.WriteLine("S2");
		}

		/* 
		text 
		*/
		
		[ShowBodyOnSlide]
		public void Sample3()
		{
			Console.WriteLine("S3");
		}

	}
}
