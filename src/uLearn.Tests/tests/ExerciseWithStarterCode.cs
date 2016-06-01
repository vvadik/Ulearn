using System;
using uLearn.CSharp;

namespace uLearn.tests
{
	[Slide("title", "8abc175fee184226b45b180dc44f7aec")]
	internal class ExerciseWithStarterCode
	{
		/*
		Add 2 and 3 please!
		*/

		[ExpectedOutput("5")]
		[Exercise]
		public void Add_2_and_3()
		{
			Console.WriteLine("Hello world!");
			throw new NotImplementedException();
			/*uncomment
			return x + y;
			*/
		}
	}
}