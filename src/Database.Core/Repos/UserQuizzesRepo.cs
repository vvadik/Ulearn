using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses.Slides.Quizzes;
using Ulearn.Core.Courses.Slides.Quizzes.Blocks;

namespace Database.Repos
{
	/* TODO (andgein): This repo is not fully migrated to .NET Core and EF Core */
	public class UserQuizzesRepo : IUserQuizzesRepo
	{
		private readonly UlearnDb db;

		public UserQuizzesRepo(UlearnDb db)
		{
			this.db = db;
		}

		public async Task<UserQuiz> AddUserQuiz(string courseId, bool isRightAnswer, string itemId, string quizId, Guid slideId, string text, string userId, DateTime time, int quizBlockScore, int quizBlockMaxScore)
		{
			var userQuiz = new UserQuiz
			{
				CourseId = courseId,
				SlideId = slideId,
				IsRightAnswer = isRightAnswer,
				ItemId = itemId,
				QuizId = quizId,
				Text = text,
				Timestamp = time,
				UserId = userId,
				QuizBlockScore = quizBlockScore,
				QuizBlockMaxScore = quizBlockMaxScore
			};
			db.UserQuizzes.Add(userQuiz);
			await db.SaveChangesAsync();
			return userQuiz;
		}

		public bool IsWaitingForManualCheck(string courseId, Guid slideId, string userId)
		{
			return db.ManualQuizCheckings.Any(c => c.CourseId == courseId && c.SlideId == slideId && c.UserId == userId && !c.IsChecked);
		}

		public bool IsQuizSlidePassed(string courseId, string userId, Guid slideId)
		{
			return db.UserQuizzes.Any(x => x.CourseId == courseId && x.UserId == userId && x.SlideId == slideId && !x.isDropped);
		}

		public IEnumerable<bool> GetQuizDropStates(string courseId, string userId, Guid slideId)
		{
			return db.UserQuizzes
				.Where(x => x.CourseId == courseId && x.UserId == userId && x.SlideId == slideId)
				.DistinctBy(q => q.Timestamp)
				.Select(q => q.isDropped);
		}

		public HashSet<Guid> GetIdOfQuizPassedSlides(string courseId, string userId)
		{
			return new HashSet<Guid>(db.UserQuizzes.Where(x => x.CourseId == courseId && x.UserId == userId).Select(x => x.SlideId).Distinct());
		}

		public HashSet<Guid> GetIdOfQuizSlidesScoredMaximum(string courseId, string userId)
		{
			var passedQuizzes = GetIdOfQuizPassedSlides(courseId, userId);
			var notScoredMaximumQuizzes = db.UserQuizzes.Where(x => x.CourseId == courseId && x.UserId == userId && x.QuizBlockScore != x.QuizBlockMaxScore).Select(x => x.SlideId).Distinct();
			passedQuizzes.ExceptWith(notScoredMaximumQuizzes);
			return passedQuizzes;
		}

		public Dictionary<string, List<UserQuiz>> GetAnswersForShowOnSlide(string courseId, QuizSlide slide, string userId)
		{
			if (slide == null)
				return null;
			var answer = new Dictionary<string, List<UserQuiz>>();
			foreach (var block in slide.Blocks.OfType<AbstractQuestionBlock>())
			{
				var ans = db.UserQuizzes
					.Where(x => x.CourseId == courseId && x.UserId == userId && x.SlideId == slide.Id && x.QuizId == block.Id && !x.isDropped)
					.OrderBy(x => x.Id)
					.ToList();
				answer[block.Id] = ans;
			}
			return answer;
		}

		public int GetAverageStatistics(Guid slideId, string courseId)
		{
			var newA = db.UserQuizzes
							.Where(x => x.CourseId == courseId && x.SlideId == slideId)
							.GroupBy(x => x.UserId)
							.Select(x => x
								.GroupBy(y => y.QuizId)
								.Select(y => y.All(z => z.QuizBlockScore == z.QuizBlockMaxScore))
								.Select(y => y ? 1 : 0)
								.DefaultIfEmpty()
								.Average())
							.DefaultIfEmpty()
							.Average() * 100;
			return (int)newA;
		}

		public int GetSubmitQuizCount(Guid slideId, string courseId)
		{
			return db.UserQuizzes.Where(x => x.CourseId == courseId && x.SlideId == slideId).Select(x => x.User).Distinct().Count();
		}

		public async Task RemoveAnswers(string courseId, string userId, Guid slideId)
		{
			var answersToRemove = db.UserQuizzes.Where(q => q.CourseId == courseId && q.UserId == userId && q.SlideId == slideId).ToList();
			db.UserQuizzes.RemoveRange(answersToRemove);
			await db.SaveChangesAsync();
		}

		public async Task DropQuizAsync(string courseId, string userId, Guid slideId)
		{
			var quizzes = db.UserQuizzes.Where(q => q.CourseId == courseId && q.UserId == userId && q.SlideId == slideId).ToList();
			foreach (var q in quizzes)
			{
				q.isDropped = true;
			}
			await db.SaveChangesAsync();
		}
		
		public void DropQuiz(string courseId, string userId, Guid slideId)
		{
			var quizzes = db.UserQuizzes.Where(q => q.CourseId == courseId && q.UserId == userId && q.SlideId == slideId).ToList();
			foreach (var q in quizzes)
			{
				q.isDropped = true;
			}
			db.SaveChanges();
		}

		public Dictionary<string, int> GetQuizBlocksTruth(string courseId, string userId, Guid slideId)
		{
			return db.UserQuizzes
				.Where(q => q.CourseId == courseId && q.UserId == userId && q.SlideId == slideId && !q.isDropped)
				.DistinctBy(q => q.QuizId)
				.ToDictionary(q => q.QuizId, q => q.QuizBlockScore);
		}

		public bool IsQuizScoredMaximum(string courseId, string userId, Guid slideId)
		{
			return db.UserQuizzes
				.Where(q => q.CourseId == courseId && q.UserId == userId && q.SlideId == slideId && !q.isDropped)
				.All(q => q.QuizBlockScore == q.QuizBlockMaxScore);
		}

		public Dictionary<string, List<UserQuiz>> GetAnswersForUser(string courseId, Guid slideId, string userId)
		{
			return db.UserQuizzes
				.Where(ans => ans.CourseId == courseId && ans.UserId == userId && ans.SlideId == slideId && !ans.isDropped)
				.ToLookup(ans => ans.QuizId)
				.ToDictionary(g => g.Key, g => g.ToList());
		}

		public ManualQuizChecking FindManualQuizChecking(string courseId, Guid slideId, string userId)
		{
			return db.ManualQuizCheckings
				.Where(i => i.CourseId == courseId && i.SlideId == slideId && i.UserId == userId && !i.IsChecked)
				.OrderByDescending(i => i.Timestamp)
				.FirstOrDefault();
		}

		public async Task SetScoreForQuizBlock(string courseId, string userId, Guid slideId, string blockId, int score)
		{
			await db.UserQuizzes
				.Where(q => q.CourseId == courseId && q.UserId == userId && q.SlideId == slideId && q.QuizId == blockId)
				.ForEachAsync(q => q.QuizBlockScore = score);
			await db.SaveChangesAsync();
		}

		public async Task RemoveUserQuizzes(string courseId, Guid slideId, string userId)
		{
			db.UserQuizzes.RemoveRange(
				db.UserQuizzes.Where(
					q => q.CourseId == courseId && q.SlideId == slideId && q.UserId == userId && !q.isDropped
				)
			);
			await db.SaveChangesAsync();
		}

		public Dictionary<string, int> GetAnswersFrequencyForChoiceBlock(string courseId, Guid slideId, string quizId)
		{
			var answers = db.UserQuizzes.Where(q => q.CourseId == courseId && q.SlideId == slideId && q.QuizId == quizId);
			var totalTries = answers.Select(q => new { q.UserId, q.Timestamp }).Distinct().Count();
			/* Don't call GroupBy().ToDictionary() because of performance issues.
			   See http://code-ninja.org/blog/2014/07/24/entity-framework-never-call-groupby-todictionary/ for details */
			return answers
				.GroupBy(q => q.ItemId)
				.Select(g => new { g.Key, Count = g.Count() })
				.ToDictionary(p => p.Key, p => p.Count.PercentsOf(totalTries));
		}
	}
}