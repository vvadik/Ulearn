using System;
#pragma warning disable 0649

namespace uLearn.tests
{
	[Slide("title", "8abc175fee184226b45b180dc44f7aec")]
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
