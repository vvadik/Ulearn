using System;
using uLearn.CSharp;

namespace uLearn.tests
{
	[Slide("title", "id")]
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