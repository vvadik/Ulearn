using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.tests
{
	[Slide("title", "id")]
	public class SampleClass
	{
		[ShowBodyOnSlide]
		public class Point
		{
			[ExcludeFromSolution]
			public Point(int x, int y)
			{
				X = x;
				Y = y;
			}

			[ExcludeFromSolution]
			public int X, Y;

			[ExpectedOutput("")]
			public void M()
			{
				
			}
		}
	}
}