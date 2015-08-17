using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uLearn.Web.Models
{
	public enum SubmissionStatus
	{
		Done = 0,
		Waiting = 1,
		NotFound = 2,
		AccessDeny = 3,
		Error = 4,
		Running = 5,
		RequestTimeLimit = 6
	}

	public class UserSolution : ISlideAction
	{
		[Required]
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(64)]
		public string CourseId { get; set; }

		[Required]
		[StringLength(64)]
		[Index("AcceptedList", 1)]
		public string SlideId { get; set; }

		public virtual ApplicationUser User { get; set; }

		[StringLength(64)]
		public string UserId { get; set; }

		public virtual TextBlob SolutionCode { get; set; }

		[StringLength(40)]
		[Required]
		public string SolutionCodeHash { get; set; }

		[Required]
		[Index("AcceptedList", 3)]
		public int CodeHash { get; set; }

		public virtual IList<Like> Likes { get; set; }

		[Required]
		[Index("AcceptedList", 4)]
		[Index("IDX_UserSolution_Timestamp")]
		public DateTime Timestamp { get; set; }

		public TimeSpan? Elapsed { get; set; }

		public SubmissionStatus Status { get; set; }

		public string DisplayName { get; set; }

		[Required]
		[Index("AcceptedList", 2)]
		public bool IsRightAnswer { get; set; }

		[Required]
		public bool IsCompilationError { get; set; }

		public virtual TextBlob CompilationError { get; set; }

		[StringLength(40)]
		public string CompilationErrorHash { get; set; }

		public virtual TextBlob Output { get; set; }

		[StringLength(40)]
		public string OutputHash { get; set; }

		[StringLength(40)]
		public string ExecutionServiceName { get; set; }

		public string GetVerdict()
		{
			if (IsCompilationError) return "CompilationError";
			if (!IsRightAnswer) return "Wrong Answer";
			
			return "Accepted";
		}
	}
}