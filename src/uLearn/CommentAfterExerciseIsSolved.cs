using System;

namespace uLearn
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class CommentAfterExerciseIsSolved : Attribute
	{
		public string Comment { get; set; }

		public CommentAfterExerciseIsSolved(string comment)
		{
			Comment = comment;
		}
	}
}