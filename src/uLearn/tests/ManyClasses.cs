using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.tests
{
	[Slide("title", "guid")]
	class ManyClasses1
	{
		void M0()
		{
			Console.WriteLine(42);
		}
		
	}

	[ShowBodyOnSlide]
	class ManyClasses2
	{
		void M()
		{
			Console.WriteLine(42);
		}
	}

	class ManyClasses3
	{
	}
}
