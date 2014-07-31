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
			public int X, Y;
		}
	}
}