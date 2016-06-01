namespace uLearn.tests
{
	[Slide("title", "8abc175fee184226b45b180dc44f7aec")]
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