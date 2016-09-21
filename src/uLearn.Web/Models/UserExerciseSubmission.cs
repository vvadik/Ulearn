using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uLearn.Web.Models
{
	public class UserExerciseSubmission : ISlideAction
	{
		[Required]
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[StringLength(40)]
		[Index("IDX_UserExerciseSubmissions_BySlideAndUser", 3)]
		public string UserId { get; set; }

		public virtual ApplicationUser User { get; set; }

		[Required]
		[StringLength(40)]
		[Index("IDX_UserExerciseSubmissions_BySlideAndUser", 1)]
		public string CourseId { get; set; }

		[Required]
		[Index("IDX_UserExerciseSubmissions_BySlideAndUser", 2)]
		public Guid SlideId { get; set; }

		[Required]
		public DateTime Timestamp { get; set; }

		[Required]
		[StringLength(40)]
		public string SolutionCodeHash { get; set; }

		public virtual TextBlob SolutionCode { get; set; }

		[Required]
		public int CodeHash { get; set; }

		public virtual IList<Like> Likes { get; set; }

		[Required]
		public virtual AutomaticExerciseChecking AutomaticChecking { get; set; }

		public virtual IList<ManualExerciseChecking> ManualCheckings { get; set; }

		public bool IsWebSubmission => CourseId == "web" && SlideId == Guid.Empty;
	}
}