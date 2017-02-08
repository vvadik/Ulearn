using System;

namespace uLearn.Web.Models
{
	public interface ISlideAction
	{
		int Id { get; }
		Guid SlideId { get; }
		string UserId { get; }
	}

	public interface ITimedSlideAction : ISlideAction
	{
		DateTime Timestamp { get; }
	}
}