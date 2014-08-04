namespace uLearn.tests
{
	[Slide("title", "id")]
	public class NestedClass
	{
		public class Point
		{
			[ExcludeFromSolution]
			public Point(int x, int y)
			{
				X = x;
				Y = y;
			}


			[ExcludeFromSolution]
			public int X, Y;

			[HideOnSlide]
			public Point()
			{
				HiddenField = 42;
			}

			[HideOnSlide]
			public int HiddenField;

			[HideOnSlide]
			public void HiddenMethod()
			{
			}
		}

		[ExpectedOutput("")]
		public void Main()
		{

		}
	}
}