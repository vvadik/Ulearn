using System;
using System.Collections.Generic;
using System.Linq;
using uLearn.Quizes;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class QuizzesRepo
	{
		private readonly ULearnDb db;
		private readonly UserQuizzesRepo userQuizzesRepo = new UserQuizzesRepo();

		public QuizzesRepo() : this(new ULearnDb())
		{

		}

		public QuizzesRepo(ULearnDb db)
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

				if (lastQuizVersion == null)
					userQuizzesRepo.UpdateQuizVersions(slideId, null, newQuizVersion.Id);

				return newQuizVersion;
			}

			return lastQuizVersion;
		}
	}
}