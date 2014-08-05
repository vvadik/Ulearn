using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using NUnit.Framework;
using uLearn.Web.Migrations;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class UserQuizzesRepo
	{
		private readonly ULearnDb db;

		public UserQuizzesRepo() : this(new ULearnDb())
		{
			
		}

		public UserQuizzesRepo(ULearnDb db)
		{
			this.db = db;
		}

		public async Task<UserQuiz> AddUserQuiz(string courseId, bool isRightAnswer, string itemId, string quizId, string slideId, string text, string userId)
		{
			var userQuiz = new UserQuiz
			{
				CourseId = courseId,
				IsRightAnswer = isRightAnswer,
				ItemId = itemId,
				QuizId = quizId,
				SlideId = slideId,
				Text = text,
				Timestamp = DateTime.Now,
				UserId = userId
			};
			db.UserQuizzes.Add(userQuiz);
			await db.SaveChangesAsync();
			return userQuiz;
		}

		public bool IsAllQuizzesPassed(string courseId, string slideId, string userId)
		{
			var quiz = db.UserQuizzes.Where(x => x.CourseId == courseId && x.UserId == userId && x.SlideId == slideId).ToList();
			return quiz.Count != 0 && quiz.All(x => x.IsRightAnswer);
		}

		public List<UserQuiz> GetQuizAnswers(string courseId, string slideId, string userId, string quizId)
		{
			return db.UserQuizzes.Where(x => x.CourseId == courseId && x.UserId == userId && x.SlideId == slideId && x.QuizId == quizId).ToList();
		}
	}
}