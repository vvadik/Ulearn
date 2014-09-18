using System;
using System.ComponentModel.DataAnnotations;

namespace uLearn.Web.Models
{
	public interface ISlideAction
	{
		int Id { get; }
		string SlideId { get; }
		string UserId { get; }
		DateTime Timestamp { get; }
	}

	public class UserQuiz : ISlideAction
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
		[Required]
		public string UserId { get; set; }

		[StringLength(64)]
		public string QuizId { get; set; }

		[StringLength(64)]
		public string ItemId { get; set; }

		[StringLength(1024)]
		public string Text { get; set; }

		[Required]
		public DateTime Timestamp { get; set; }

		
		[Required]
		//Корректный ли вариант ItemId
		public bool IsRightAnswer { get; set; }

		[Required]
		//Корректный ли весь ответ на вопрос? Отличается от IsRightAnswer для вопросов с множественным выбором
		public bool IsRightQuizBlock { get; set; }
	}
}