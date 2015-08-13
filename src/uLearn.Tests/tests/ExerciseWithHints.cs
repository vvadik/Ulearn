using System;

namespace uLearn.tests
{
	[Slide("title", "id")]
	internal class ExerciseWithHints
	{
		[ExpectedOutput("5")]
		[Hint("hint1")]
		[Hint("hint2")]
		public void Add_2_and_3()
		{
			throw new NotImplementedException();
		}
	}
}