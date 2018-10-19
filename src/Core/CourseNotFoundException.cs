using System;

namespace uLearn
{
	public class CourseNotFoundException : Exception
	{
		public CourseNotFoundException(string courseId)
			: base($"Course \"{courseId}\" not found")
		{
		}

		public CourseNotFoundException(string courseId, Exception innerException)
			: base($"Course \"{courseId}\" not found", innerException)
		{
		}
	}
}