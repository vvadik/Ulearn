using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLessons.tests
{
	internal class CommentsInsideSample
	{
		[Sample]
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