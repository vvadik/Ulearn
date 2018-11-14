using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using uLearn;
using uLearn.Courses;
using uLearn.Courses.Slides.Quizzes;
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
		[Index("IDX_QuizVersion_QuizVersionBySlide")]
		[Index("IDX_QuizVersion_QuizVersionBySlideAndTime", 1)]
		public Guid SlideId { get; set; }

		[Required]
		public string NormalizedXml { get; set; }

		[Required]
		[Index("IDX_QuizVersion_QuizVersionBySlideAndTime", 2)]
		public DateTime LoadingTime { get; set; }

		public Quiz GetRestoredQuiz(Course course, Unit unit)
		{
			var quiz = NormalizedXml.DeserializeXml<Quiz>().InitQuestionIndices();
			QuizSlideLoader.BuildUp(quiz, unit, course.Id, course.Settings);
			return quiz;
		}
	}
}