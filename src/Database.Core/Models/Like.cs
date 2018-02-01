using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
	public class Like
	{
		[Key]
		public int Id { get; set; }

		public virtual UserExerciseSubmission Submission { get; set; }

		[Required]
		public int SubmissionId { get; set; }

		[Required]
		[StringLength(64)]
		public string UserId { get; set; }

		public virtual ApplicationUser User { get; set; }

		[Required]
		public DateTime Timestamp { get; set; }
	}
}