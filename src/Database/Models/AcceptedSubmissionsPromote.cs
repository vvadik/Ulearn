using System.ComponentModel.DataAnnotations;

namespace Database.Models
{
	public class AcceptedSolutionsPromote
	{
		[Key]
		public int SubmissionId { get; set; }

		public virtual UserExerciseSubmission Submission { get; set; }

		[Required]
		public string UserId { get; set; }

		public virtual ApplicationUser User { get; set; }
	}
}