using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using uLearn.Courses.Slides.Quizzes;
using uLearn.Extensions;
using Ulearn.Common.Extensions;

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
		public Guid SlideId { get; set; }

		[Required]
		public string NormalizedXml { get; set; }

		[Required]
		public DateTime LoadingTime { get; set; }

		[NotMapped]
		public Quiz RestoredQuiz => NormalizedXml.DeserializeXml<Quiz>().InitQuestionIndices();
	}
}