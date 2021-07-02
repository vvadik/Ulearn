using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
	public class AcceptedSolutionsPromote
	{
		[Key]
		[ForeignKey("Submission")]
		public int SubmissionId { get; set; }

		public virtual UserExerciseSubmission Submission { get; set; }

		[Required]
		public string UserId { get; set; }

		public virtual ApplicationUser User { get; set; }

		[Required]
		public DateTime Timestamp { get; set; }
	}
}