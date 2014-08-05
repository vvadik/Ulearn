using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using NUnit.Framework;
using uLearn.Quizes;
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

		public async Task<UserQuiz> AddUserQuiz(string courseId, bool isRightAnswer, string itemId, string quizId, string slideId, string text, string userId, DateTime time)
		{
			var userQuiz = new UserQuiz
			{
				CourseId = courseId,
				IsRightAnswer = isRightAnswer,
				ItemId = itemId,
				QuizId = quizId,
				SlideId = slideId,
				Text = text,
				Timestamp = time,
				UserId = userId
			};
			db.UserQuizzes.Add(userQuiz);
			await db.SaveChangesAsync();
			return userQuiz;
		}

		public bool IsQuizPassed(string courseId, string slideId, string userId)
		{
			return db.UserQuizzes.Any(x => x.CourseId == courseId && x.UserId == userId && x.SlideId == slideId);
		}

		public Dictionary<string, List<string>> GetAnswersForShowOnSlide(string courseId, QuizSlide slide, string userId)
		{
			if (slide == null)
				return null;
			var answer = new Dictionary<string, List<string>>();
			foreach (var block in slide.Quiz.Blocks)
			{
				var ans = db.UserQuizzes
					.Where(x => x.UserId == userId && x.CourseId == courseId && x.SlideId == slide.Id && x.QuizId == block.Id).ToList();
				if (block is ChoiceBlock)
					answer[block.Id] = ans.Select(x => x.ItemId).ToList();
				else answer[block.Id] = ans.Select(x => x.Text).ToList();
			}
			return answer;
		}
	}
}