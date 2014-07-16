using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace uLearn.Web.Models
{
	public class UserSolution
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
		[StringLength(1024)]
		public string Code { get; set; }

		[Required]
		public int CodeHash { get; set; }

		public virtual IList<Like> LikersStorage { get; set; }

		[Required]
		public DateTime Timestamp { get; set; }

		[Required]
		public bool IsRightAnswer { get; set; }

		[Required]
		public bool IsCompilationError { get; set; }

		[StringLength(1024)]
		public string CompilationError { get; set; }

		[StringLength(1024)]
		public string Output { get; set; }
	}
}