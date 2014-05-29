using System;

namespace uLearn
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class ExerciseAttribute : Attribute
	{
		public bool SingleStatement;
	}
}