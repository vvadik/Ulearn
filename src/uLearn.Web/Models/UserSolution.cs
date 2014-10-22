using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uLearn.Web.Models
{
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
		public string SlideId { get; set; }

		public virtual ApplicationUser User { get; set; }

		[StringLength(64)]
		public string UserId { get; set; }

		[Required]
		[StringLength(4096)]
		public string Code { get; set; }

		[Required]
		public int CodeHash { get; set; }

		public virtual IList<Like> Likes { get; set; }

		[Required]
		public DateTime Timestamp { get; set; }

		[Required]
		[Index]
		public bool IsRightAnswer { get; set; }

		[Required]
		public bool IsCompilationError { get; set; }

		[StringLength(4096)]
		public string CompilationError { get; set; }

		[StringLength(4096)]
		public string Output { get; set; }

		public string GetVerdict()
		{
			if (IsCompilationError) return "CompilationError";
			if (!IsRightAnswer) return "Wrong Answer";
			
			return "Accepted";
		}
	}
}