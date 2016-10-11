using System;
using System.Runtime.Serialization;

namespace uLearn
{
	[Serializable]
	public class SlideNotFoundException : Exception
	{
		public SlideNotFoundException()
		{
		}

		public SlideNotFoundException(string message) : base(message)
		{
		}

		public SlideNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected SlideNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}