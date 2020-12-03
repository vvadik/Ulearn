using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Database.Models.Quizzes;

namespace Database.Repos
{
	public interface ISlideCheckingsRepo
	{
		Task AddQuizAttemptForManualChecking(UserQuizSubmission submission, string courseId, Guid slideId, string userId);
		Task AddQuizAttemptWithAutomaticChecking(string courseId, Guid slideId, string userId, int automaticScore);
		IEnumerable<ManualExerciseChecking> GetUsersPassedManualExerciseCheckings(string courseId, string userId);
		Task<ManualExerciseChecking> AddManualExerciseChecking(string courseId, Guid slideId, string userId, UserExerciseSubmission submission);
		Task<Dictionary<string, List<Guid>>> GetSlideIdsWaitingForManualExerciseCheckAsync(string courseId, IEnumerable<string> userIds);
		Task RemoveWaitingManualExerciseCheckings(string courseId, Guid slideId, string userId);
		Task RemoveAttempts(string courseId, Guid slideId, string userId, bool saveChanges = true);
		bool IsSlidePassed(string courseId, Guid slideId, string userId);
		int GetManualScoreForSlide(string courseId, Guid slideId, string userId);
		Dictionary<string, int> GetManualScoresForSlide(string courseId, Guid slideId, List<string> userIds);
		int GetAutomaticScoreForSlide(string courseId, Guid slideId, string userId);
		Dictionary<string, int> GetAutomaticScoresForSlide(string courseId, Guid slideId, List<string> userIds);
		Task<IEnumerable<ManualExerciseChecking>> GetManualExerciseCheckingQueueAsync(ManualCheckingQueueFilterOptions options);
		T FindManualCheckingById<T>(int id) where T : AbstractManualSlideChecking;
		bool IsProhibitedToSendExerciseToManualChecking(string courseId, Guid slideId, string userId);
		Task LockManualChecking<T>(T checkingItem, string lockedById) where T : AbstractManualSlideChecking;
		Task ProhibitFurtherExerciseManualChecking(ManualExerciseChecking checking);
		Task ResetManualCheckingLimitsForUser(string courseId, string userId);
		Task ResetAutomaticCheckingLimitsForUser(string courseId, string userId);
		Task DisableProhibitFurtherManualCheckings(string courseId, string userId);
		Task NotCountOldAttemptsToQuizzesWithManualChecking(string courseId, string userId);
		Task NotCountOldAttemptsToQuizzesWithAutomaticChecking(string courseId, string userId);
		Task<ExerciseCodeReview> AddExerciseCodeReview(ManualExerciseChecking checking, string userId, int startLine, int startPosition, int finishLine, int finishPosition, string comment, bool setAddingTime = true);
		Task<ExerciseCodeReview> AddExerciseCodeReview(UserExerciseSubmission submission, string userId, int startLine, int startPosition, int finishLine, int finishPosition, string comment, bool setAddingTime = false);
		ExerciseCodeReview FindExerciseCodeReviewById(int reviewId);
		Task DeleteExerciseCodeReview(ExerciseCodeReview review);
		Task UpdateExerciseCodeReview(ExerciseCodeReview review, string newComment);
		Dictionary<int, List<ExerciseCodeReview>> GetExerciseCodeReviewForCheckings(IEnumerable<int> checkingsIds);
		List<string> GetTopUserReviewComments(string courseId, Guid slideId, string userId, int count);
		Task HideFromTopCodeReviewComments(string courseId, Guid slideId, string userId, string comment);
		List<ExerciseCodeReview> GetAllReviewComments(string courseId, Guid slideId);
	}
}