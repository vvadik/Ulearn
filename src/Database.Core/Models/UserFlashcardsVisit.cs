using System;
using System.ComponentModel.DataAnnotations;

namespace Database.Models
{
	public class UserFlashcardsVisit
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string UserId { get; set; }
		public virtual ApplicationUser User { get; set; }
		
		[Required]
		[StringLength(64)]
		public string CourseId { get; set; }

		[Required]
		public Guid UnitId { get; set; }

		[Required]
		[StringLength(64)]
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