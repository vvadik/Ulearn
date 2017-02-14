using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uLearn.Web.Models
{
	public class UserQuiz : ITimedSlideAction
	{
		[Required]
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(64)]
		public string CourseId { get; set; }

		[Required]
		[Index("FullIndex", 2)]
		[Index("StatisticsIndex", 1)]
		public Guid SlideId { get; set; }

		public virtual QuizVersion QuizVersion { get; set; }
		
		public int? QuizVersionId { get; set; }

		public virtual ApplicationUser User { get; set; }

		[StringLength(64)]
		[Required]
		[Index("FullIndex", 1)]
		public string UserId { get; set; }

		[StringLength(64)]
		[Index("FullIndex", 4)]
		public string QuizId { get; set; }

		[StringLength(64)]
		[Index("FullIndex", 5)]
		public string ItemId { get; set; }

		[StringLength(1024)]
		public string Text { get; set; }

		[Required]
		[Index("StatisticsIndex", 2)]
		public DateTime Timestamp { get; set; }

		[Required]
		[Index("FullIndex", 3)]
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

		public bool IsQuizBlockScoredMaximum
		{
			get { return QuizBlockScore == QuizBlockMaxScore; }
		}
	}
}