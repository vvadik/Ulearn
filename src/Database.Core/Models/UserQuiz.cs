using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
	/* For backward compatibility: EF Core changed table naming convention.
	   See https://github.com/aspnet/Announcements/issues/167 for details */
	[Table("UserQuizs")]
	public class UserQuiz : ITimedSlideAction
	{
		[Required]
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(64)]
		public string CourseId { get; set; }

		[Required]
		public Guid SlideId { get; set; }

		public virtual QuizVersion QuizVersion { get; set; }

		public int? QuizVersionId { get; set; }

		public virtual ApplicationUser User { get; set; }

		[StringLength(64)]
		[Required]
		public string UserId { get; set; }

		[StringLength(64)]
		public string QuizId { get; set; }

		[StringLength(64)]
		public string ItemId { get; set; }

		[StringLength(8192)]
		public string Text { get; set; }

		[Required]
		public DateTime Timestamp { get; set; }

		[Required]
		public bool isDropped { get; set; }


		[Required]
		// Корректный ли вариант ItemId
		public bool IsRightAnswer { get; set; }

		[Required]
		// Количество баллов за весь блок
		public int QuizBlockScore { get; set; }

		[Required]
		// Максимально возможное количество баллов за весь блок
		public int QuizBlockMaxScore { get; set; }

		public bool IsQuizBlockScoredMaximum => QuizBlockScore == QuizBlockMaxScore;
	}
}