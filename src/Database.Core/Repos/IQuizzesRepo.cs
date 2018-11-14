using System;
using System.Collections.Generic;
using Database.Models;
using uLearn.Courses.Slides.Quizzes;

namespace Database.Repos
{
	public interface IQuizzesRepo
	{
		IEnumerable<QuizVersion> GetQuizVersions(string courseId, Guid slideId);
		QuizVersion GetLastQuizVersion(string courseId, Guid slideId);
		QuizVersion GetFirstQuizVersion(string courseId, Guid slideId);
		QuizVersion AddQuizVersionIfNeeded(string courseId, QuizSlide slide);
	}
}