using System;
using System.ComponentModel.DataAnnotations;

namespace uLearn.Web.Models
{
	public class UserSolution
	{
		[Required]
		[Key]
		public int SolutionId { get; set; }

		[Required]
		[StringLength(64)]
		public string CourseId { get; set; }

		[Required]
		[StringLength(64)]
		public virtual ApplicationUser User { get; set; }

		[Required]
		[StringLength(64)]
		public string SlideId { get; set; }

		[Required]
		[StringLength(1024)]
		public string Code { get; set; }

		[Required]
		public DateTime Timestamp { get; set; }

		[Required]
		public bool IsRightAnswer { get; set; }

		[Required]
		public bool IsCompilationError { get; set; }

		[Required]
		[StringLength(1024)]
		public string CompilationError { get; set; }

		[Required]
		[StringLength(1024)]
		public string Output { get; set; }
	}
}