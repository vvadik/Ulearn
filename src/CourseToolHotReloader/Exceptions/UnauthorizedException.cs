using System;

namespace CourseToolHotReloader.Exceptions
{
	public class UnauthorizedException : Exception
	{
		public UnauthorizedException(string message)
			: base(message)
		{ }
	}
}