using System;
using System.Collections.Generic;
using System.Linq;
using Database.Models;
using Ulearn.Core.Courses.Slides.Quizzes;

namespace Database.Repos
{
	/* TODO (andgein): This repo is not fully migrated to .NET Core and EF Core */
	public class QuizzesRepo : IQuizzesRepo
	{
		private readonly UlearnDb db;

		public QuizzesRepo(UlearnDb db)
		{
			this.db = db;
		}

		public IEnumerable<QuizVersion> GetQuizVersions(string courseId, Guid slideId)
		{
			return db.QuizVersions.Where(v => v.CourseId == courseId && v.SlideId == slideId).OrderByDescending(v => v.LoadingTime);
		}

		public QuizVersion GetLastQuizVersion(string courseId, Guid slideId)
		{
			return db.QuizVersions.Where(v => v.CourseId == courseId && v.SlideId == slideId).OrderByDescending(v => v.LoadingTime).FirstOrDefault();
		}

		public QuizVersion GetFirstQuizVersion(string courseId, Guid slideId)
		{
			return db.QuizVersions.Where(v => v.CourseId == courseId && v.SlideId == slideId).OrderBy(v => v.LoadingTime).FirstOrDefault();
		}

		public QuizVersion AddQuizVersionIfNeeded(string courseId, QuizSlide slide)
		{
			var slideId = slide.Id;

			var quizXml = slide.QuizNormalizedXml;
			var lastQuizVersion = GetLastQuizVersion(courseId, slideId);
			var newQuizVersion = new QuizVersion
			{
				CourseId = courseId,
				SlideId = slideId,
				LoadingTime = DateTime.Now,
				NormalizedXml = quizXml
			};

			if (lastQuizVersion == null || lastQuizVersion.NormalizedXml != newQuizVersion.NormalizedXml)
			{
				db.QuizVersions.Add(newQuizVersion);
				db.SaveChanges();

				return newQuizVersion;
			}

			return lastQuizVersion;
		}
	}
}