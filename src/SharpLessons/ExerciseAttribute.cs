using System;

namespace SharpLessons
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class ExerciseAttribute : Attribute
	{
		public bool SingleStatement;
	}
}