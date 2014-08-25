using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security.Provider;
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

		public async Task<UserQuiz> AddUserQuiz(string courseId, bool isRightAnswer, string itemId, string quizId, string slideId, string text, string userId, DateTime time, bool isRightQuizBlock)
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
				UserId = userId,
				IsRightQuizBlock = isRightQuizBlock
			};
			db.UserQuizzes.Add(userQuiz);
			await db.SaveChangesAsync();
			return userQuiz;
		}

		public bool IsQuizSlidePassed(string courseId, string userId, string slideId)
		{
			return db.UserQuizzes.Any(x => x.UserId == userId && x.SlideId == slideId && x.CourseId == courseId);
		}

		public HashSet<string> GetIdOfQuizPassedSlides(string courseId, string userId)
		{
			return new HashSet<string>(db.UserQuizzes.Where(x => x.CourseId == courseId && x.UserId == userId).Select(x => x.SlideId).Distinct());
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
				else if (block is IsTrueBlock)
					answer[block.Id] = ans.Select(x => x.Text).ToList();
				else if(block is FillInBlock)
					answer[block.Id] = new List<string>
					{
						ans.Select(x => x.Text).FirstOrDefault(),
						ans.Select(x => x.IsRightAnswer).FirstOrDefault().ToString()
					};
			}
			return answer;
		}

		public FillInBlockAnswerInfo GetFillInBlockAnswerInfo(string courseId, string slideId, string quizId, string userId, int questionIndex)
		{
			var answer = db.UserQuizzes.FirstOrDefault(x => x.CourseId == courseId && x.UserId == userId && x.SlideId == slideId && x.QuizId == quizId);
			return new FillInBlockAnswerInfo
			{
				Answer = answer == null ? null : answer.Text,
				IsRight = answer != null && answer.IsRightAnswer,
				Id = questionIndex.ToString()
			};
		}

		public ChoiceBlockAnswerInfo GetChoiseBlockAnswerInfo(string courseId, string slideId, ChoiceBlock block, string userId, int questionIndex)
		{
			var ans = new SortedDictionary<string, bool>();
			foreach (var item in block.Items)
			{
				ans[item.Id] = false;
			}
			foreach (var itemId in db.UserQuizzes.Where(x => x.CourseId == courseId && x.UserId == userId && x.SlideId == slideId && x.QuizId == block.Id).Select(x => x.ItemId))
			{
				ans[itemId] = true;
			}
			return new ChoiceBlockAnswerInfo
			{
				AnswersId = ans,
				Id = questionIndex.ToString(),
				RealyRightAnswer = new HashSet<string>(block.Items.Where(x => x.IsCorrect).Select(x => x.Id))
			};
		}

		public IsTrueBlockAnswerInfo GetIsTrueBlockAnswerInfo(string courseId, string slideId, string quizId, string userId, int questionIndex)
		{
			var answer =  db.UserQuizzes.FirstOrDefault(x => x.CourseId == courseId && x.UserId == userId && x.SlideId == slideId && x.QuizId == quizId);
			return new IsTrueBlockAnswerInfo
			{
				IsAnswered = answer != null,
				Answer = answer != null && answer.Text == "True",
				Id = questionIndex.ToString(),
				IsRight = answer != null && answer.IsRightAnswer
			};
		}

		public int GetAverageStatistics(string slideId, string courseId)
		{
			var newA = db.UserQuizzes
				.Where(x => x.SlideId == slideId && x.CourseId == courseId)
				.GroupBy(x => x.UserId)
				.Select(x => x
					.GroupBy(y => y.QuizId)
					.Select(y => y.All(z => z.IsRightQuizBlock))
					.Select(y => y ? 1 : 0)
					.DefaultIfEmpty()
					.Average())
				.DefaultIfEmpty()
				.Average() * 100;
			return (int) newA;
		}

		public int GetSubmitQuizCount(string slideId, string courseId)
		{
			return db.UserQuizzes.Where(x => x.SlideId == slideId && x.CourseId == courseId).Select(x => x.User).Distinct().Count();
		}

		public int GetQuizSuccessful(string courseId, string slideId, string userId)
		{
			return (int) db.UserQuizzes
				.Where(x => x.CourseId == courseId && x.SlideId == slideId && x.UserId == userId)
				.GroupBy(y => y.QuizId)
				.Select(y => y.All(z => z.IsRightQuizBlock))
				.Select(y => y ? 1 : 0)
				.DefaultIfEmpty()
				.Average() * 100;
		}
	}
}