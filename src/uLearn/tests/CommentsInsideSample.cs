using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.tests
{
	[Slide("title", "id")]
	internal class CommentsInsideSample
	{
		[ShowBodyOnSlide]
		public void DoSample()
		{
//			Not a block!

			/*uncomment
			Not a block too
			*/
			Console.WriteLine("Hello Sample!");
		}
	}
}