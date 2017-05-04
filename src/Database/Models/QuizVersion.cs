using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using uLearn.Extensions;
using uLearn.Quizes;

namespace Database.Models
{
	public class QuizVersion
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[StringLength(64)]
		public string CourseId { get; set; }

		[Required]
		[Index("IDX_QuizVersion_QuizVersionBySlide")]
		[Index("IDX_QuizVersion_QuizVersionBySlideAndTime", 1)]
		public Guid SlideId { get; set; }

		[Required]
		public string NormalizedXml { get; set; }

		[Required]
		[Index("IDX_QuizVersion_QuizVersionBySlideAndTime", 2)]
		public DateTime LoadingTime { get; set; }

		[NotMapped]
		public Quiz RestoredQuiz => NormalizedXml.DeserializeXml<Quiz>().InitQuestionIndices();
	}
}