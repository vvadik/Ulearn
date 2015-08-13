using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.tests
{
	[Slide("title", "id")]
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
