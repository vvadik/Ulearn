namespace uLearn.tests
{
	[Slide("title", "id")]
	public class HiddenNestedClass
	{
		[HideOnSlide]
		public class HiddenClass
		{
			public HiddenClass()
			{
				HiddenField = 42;
			}

			public void HiddenMethod()
			{
			}

			public int HiddenField;
		}

		[ExpectedOutput("")]
		public void Main()
		{
		}
	}
}