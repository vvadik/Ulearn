using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Models;
using Ulearn.Core.Courses.Slides.Quizzes;

namespace Database.Repos
{
	public interface IUserQuizzesRepo
	{
		Task<UserQuiz> AddUserQuiz(string courseId, bool isRightAnswer, string itemId, string quizId, Guid slideId, string text, string userId, DateTime time, int quizBlockScore, int quizBlockMaxScore);
		bool IsWaitingForManualCheck(string courseId, Guid slideId, string userId);
		bool IsQuizSlidePassed(string courseId, string userId, Guid slideId);
		IEnumerable<bool> GetQuizDropStates(string courseId, string userId, Guid slideId);
		HashSet<Guid> GetIdOfQuizPassedSlides(string courseId, string userId);
		HashSet<Guid> GetIdOfQuizSlidesScoredMaximum(string courseId, string userId);
		Dictionary<string, List<UserQuiz>> GetAnswersForShowOnSlide(string courseId, QuizSlide slide, string userId);
		Task RemoveAnswers(string courseId, string userId, Guid slideId);
		Task DropQuizAsync(string courseId, string userId, Guid slideId);
		void DropQuiz(string courseId, string userId, Guid slideId);
		Dictionary<string, int> GetQuizBlocksTruth(string courseId, string userId, Guid slideId);
		bool IsQuizScoredMaximum(string courseId, string userId, Guid slideId);
		Dictionary<string, List<UserQuiz>> GetAnswersForUser(string courseId, Guid slideId, string userId);
		ManualQuizChecking FindManualQuizChecking(string courseId, Guid slideId, string userId);
		Task SetScoreForQuizBlock(string courseId, string userId, Guid slideId, string blockId, int score);
		Task RemoveUserQuizzes(string courseId, Guid slideId, string userId);
		Dictionary<string, int> GetAnswersFrequencyForChoiceBlock(string courseId, Guid slideId, string quizId);
	}
}