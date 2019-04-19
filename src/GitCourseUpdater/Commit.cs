using System;

namespace GitCourseUpdater
{
	public class CommitInfo
	{
		public string Hash;
		public string Message;
		public string AuthorName;
		public string AuthorEmail;
		public DateTimeOffset Time;
	}
}