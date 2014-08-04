using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.tests
{
	[Slide("title", "id")]
	class Sample
	{
		[ShowBodyOnSlide]
		public void HiddenMethodHeader()
		{
			Console.WriteLine("Hello Sample!");
		}

		public void Method()
		{
		}

		[HideOnSlide]
		public void HiddenMethod() { }

		[HideOnSlide] 
		public int HiddenField;
		
		public int Field;
	}
}
