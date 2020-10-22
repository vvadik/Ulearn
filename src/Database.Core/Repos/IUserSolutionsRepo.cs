using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using uLearn;
using Ulearn.Common;
using Ulearn.Core;
using Ulearn.Core.RunCheckerJobApi;
using Ulearn.Core.Telegram;

namespace Database.Repos
{
	public interface IUserSolutionsRepo
	{
		Task<UserExerciseSubmission> AddUserExerciseSubmission(
			string courseId, Guid slideId,
			string code, string compilationError, string output,
			string userId, string executionServiceName, string displayName,
			Language language,
			string sandbox,
			AutomaticExerciseCheckingStatus status = AutomaticExerciseCheckingStatus.Waiting);

		Task RemoveSubmission(UserExerciseSubmission submission);

		///<returns>(likesCount, isLikedByThisUsed)</returns>
		Task<Tuple<int, bool>> Like(int solutionId, string userId);

		IQueryable<UserExerciseSubmission> GetAllSubmissions(string courseId, bool includeManualAndAutomaticCheckings = true);
		IQueryable<UserExerciseSubmission> GetAllSubmissions(string courseId, IEnumerable<Guid> slidesIds);
		IQueryable<UserExerciseSubmission> GetAllSubmissions(string courseId, IEnumerable<Guid> slidesIds, DateTime periodStart, DateTime periodFinish);
		IQueryable<UserExerciseSubmission> GetAllAcceptedSubmissions(string courseId, IEnumerable<Guid> slidesIds, DateTime periodStart, DateTime periodFinish);
		IQueryable<UserExerciseSubmission> GetAllAcceptedSubmissions(string courseId, IEnumerable<Guid> slidesIds);
		IQueryable<UserExerciseSubmission> GetAllAcceptedSubmissions(string courseId);
		IQueryable<UserExerciseSubmission> GetAllAcceptedSubmissionsByUser(string courseId, IEnumerable<Guid> slideIds, string userId);
		IQueryable<UserExerciseSubmission> GetAllAcceptedSubmissionsByUser(string courseId, string userId);
		IQueryable<UserExerciseSubmission> GetAllAcceptedSubmissionsByUser(string courseId, Guid slideId, string userId);
		IQueryable<UserExerciseSubmission> GetAllSubmissionsByUser(string courseId, Guid slideId, string userId);
		IQueryable<UserExerciseSubmission> GetAllSubmissionsByUsers(SubmissionsFilterOptions filterOptions);
		Task<List<AcceptedSolutionInfo>> GetBestTrendingAndNewAcceptedSolutions(string courseId, List<Guid> slidesIds);
		Task<List<AcceptedSolutionInfo>> GetBestTrendingAndNewAcceptedSolutions(string courseId, Guid slideId);
		Task<int> GetAcceptedSolutionsCount(string courseId, Guid slideId);
		Task<bool> IsCheckingSubmissionByUser(string courseId, Guid slideId, string userId, DateTime periodStart, DateTime periodFinish);
		Task<HashSet<Guid>> GetIdOfPassedSlides(string courseId, string userId);
		IQueryable<UserExerciseSubmission> GetAllSubmissions(int max, int skip);
		Task<AutomaticExerciseCheckingStatus?> GetSubmissionAutomaticCheckingStatus(int id);
		Task<UserExerciseSubmission> FindSubmissionById(int id);
		Task<UserExerciseSubmission> FindSubmissionById(string id);
		Task<List<UserExerciseSubmission>> FindSubmissionsByIds(IEnumerable<int> checkingsIds);
		Task SaveResult(RunningResults result, Func<UserExerciseSubmission, Task> onSave);
		Task RunAutomaticChecking(UserExerciseSubmission submission, TimeSpan timeout, bool waitUntilChecked, int priority);
		Task<Dictionary<int, string>> GetSolutionsForSubmissions(IEnumerable<int> submissionsIds);
		Task WaitAnyUnhandledSubmissions(TimeSpan timeout);
		Task WaitUntilSubmissionHandled(TimeSpan timeout, int submissionId);
		Task<UserExerciseSubmission> GetUnhandledSubmission(string agentName, List<string> sandboxes);
	}
}