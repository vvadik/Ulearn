using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
	public class ExerciseCodeReview
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		public int ExerciseCheckingId { get; set; }

		public virtual ManualExerciseChecking ExerciseChecking { get; set; }

		[Required]
		public int StartLine { get; set; }

		[Required]
		public int StartPosition { get; set; }

		[Required]
		public int FinishLine { get; set; }

		[Required]
		public int FinishPosition { get; set; }

		[Required]
		public string Comment { get; set; }

		[Required]
		[StringLength(64)]
		public string AuthorId { get; set; }

		public virtual ApplicationUser Author { get; set; }

		[Required]
		public bool IsDeleted { get; set; }

		[Required]
		public bool HiddenFromTopComments { get; set; }
	}
}