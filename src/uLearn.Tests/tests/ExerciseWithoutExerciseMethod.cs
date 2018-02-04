namespace uLearn.tests
{
	[Slide("title", "8abc175fee184226b45b180dc44f7aec")]
	internal class ExerciseWithoutExerciseMethod
	{
		[ExpectedOutput("42")]
		public void Main()
		{
			new MyClass();
		}
	}


	[Exercise]
	class MyClass
	{
	}

	/*uncomment
	class MyClass{ }
	*/
}