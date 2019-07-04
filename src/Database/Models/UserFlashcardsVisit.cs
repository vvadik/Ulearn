using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
	public class UserFlashcardsVisit
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[Index("IDX_UserFlashcardsVisits_ByUserIdAndCourseIdAndUnitIdAndFlashcardId", 1, IsUnique = true)]
		public string UserId { get; set; }
		public virtual ApplicationUser User { get; set; }
		
		[Required]
		[StringLength(64)]
		[Index("IDX_UserFlashcardsVisits_ByUserIdAndCourseIdAndUnitIdAndFlashcardId", 2, IsUnique = true)]
		public string CourseId { get; set; }

		[Required]
		[Index("IDX_UserFlashcardsVisits_ByUserIdAndCourseIdAndUnitIdAndFlashcardId", 3, IsUnique = true)]
		public Guid UnitId { get; set; }

		[Required]
		[StringLength(64)]
		[Index("IDX_UserFlashcardsVisits_ByUserIdAndCourseIdAndUnitIdAndFlashcardId", 4, IsUnique = true)]
		public string FlashcardId { get; set; }

		[Required]
		public Score Score { get; set; }

		[Required]
		public DateTime Timestamp { get; set; }
	}

	public enum Score
	{
		NotViewed,
		One,
		Two,
		Three,
		Four,
		Five
	}
}