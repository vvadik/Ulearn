using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable 0649

namespace uLearn.tests
{
	[Slide("title", "id")]
	class Simple
	{
		[ShowBodyOnSlide]
		public void HiddenMethodHeader()
		{
			Console.WriteLine(42);
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
