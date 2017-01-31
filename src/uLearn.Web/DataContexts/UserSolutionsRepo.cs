using RunCsJob.Api;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ApprovalUtilities.Utilities;
using log4net;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class UserSolutionsRepo
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(UserSolutionsRepo));
		private readonly ULearnDb db;
		private readonly TextsRepo textsRepo = new TextsRepo();
		private readonly VisitsRepo visitsRepo = new VisitsRepo();
		private readonly CourseManager courseManager = WebCourseManager.Instance;

		public UserSolutionsRepo() : this(new ULearnDb())
		{

		}

		public UserSolutionsRepo(ULearnDb db)
		{
			this.db = db;
		}

		/* TODO(andgein): Remove isRightAnswer? */
		public async Task<UserExerciseSubmission> AddUserExerciseSubmission(string courseId, Guid slideId, string code, bool isRightAnswer, string compilationError, string output, string userId, string executionServiceName, string displayName)
		{
			if (string.IsNullOrWhiteSpace(code))
				code = "// no code";
			var hash = (await textsRepo.AddText(code)).Hash;
			var compilationErrorHash = (await textsRepo.AddText(compilationError)).Hash;
			var outputHash = (await textsRepo.AddText(output)).Hash;

			var automaticChecking = new AutomaticExerciseChecking
			{
				CourseId = courseId,
				SlideId = slideId,
				UserId = userId,
				Timestamp = DateTime.Now,

				CompilationErrorHash = compilationErrorHash,
				IsCompilationError = !string.IsNullOrWhiteSpace(compilationError),
				OutputHash = outputHash,
				ExecutionServiceName = executionServiceName,
				DisplayName = displayName,
				Status = AutomaticExerciseCheckingStatus.Waiting,
				IsRightAnswer = isRightAnswer,
			};

			db.AutomaticExerciseCheckings.Add(automaticChecking);

			var submission = new UserExerciseSubmission
			{
				CourseId = courseId,
				SlideId = slideId,
				UserId = userId,
				Timestamp = DateTime.Now,

				SolutionCodeHash = hash,
				CodeHash = code.Split('\n').Select(x => x.Trim()).Aggregate("", (x, y) => x + y).GetHashCode(),
				Likes = new List<Like>(),
				AutomaticChecking = automaticChecking,
				AutomaticCheckingIsRightAnswer = isRightAnswer,
			};
			
			db.UserExerciseSubmissions.Add(submission);

			try
			{
				await db.SaveChangesAsync();
			}
			catch (DbEntityValidationException e)
			{
				log.Error(e);
				throw new Exception(
					string.Join("\r\n",
						e.EntityValidationErrors.SelectMany(v => v.ValidationErrors).Select(err => err.PropertyName + " " + err.ErrorMessage)));
			}

			return submission;
		}

		public async Task RemoveSubmission(UserExerciseSubmission submission)
		{
			if (submission.Likes != null)
				db.SolutionLikes.RemoveRange(submission.Likes);
			if (submission.AutomaticChecking != null)
				db.AutomaticExerciseCheckings.Remove(submission.AutomaticChecking);
			if (submission.ManualCheckings != null)
				db.ManualExerciseCheckings.RemoveRange(submission.ManualCheckings);

			db.UserExerciseSubmissions.Remove(submission);
			await db.SaveChangesAsync();
		}

		///<returns>(likesCount, isLikedByThisUsed)</returns>
		public async Task<Tuple<int, bool>> Like(int solutionId, string userId)
		{
			var solutionForLike = db.UserExerciseSubmissions.Find(solutionId);
			if (solutionForLike == null) throw new Exception("Solution " + solutionId + " not found");
			var hisLike = db.SolutionLikes.FirstOrDefault(like => like.UserId == userId && like.SubmissionId == solutionId);
			var votedAlready = hisLike != null;
			var likesCount = solutionForLike.Likes.Count;
			if (votedAlready)
			{
				db.SolutionLikes.Remove(hisLike);
				likesCount--;
			}
			else
			{
				db.SolutionLikes.Add(new Like { SubmissionId = solutionId, Timestamp = DateTime.Now, UserId = userId });
				likesCount++;
			}
			await db.SaveChangesAsync();
			return Tuple.Create(likesCount, !votedAlready);
		}

		public IQueryable<UserExerciseSubmission> GetAllSubmissions(string courseId, IEnumerable<Guid> slidesIds)
		{
			return db.UserExerciseSubmissions
				.Include(s => s.ManualCheckings)
				.Where(x =>
					x.CourseId == courseId &&
					slidesIds.Contains(x.SlideId)
				);
		}

		public IQueryable<UserExerciseSubmission> GetAllSubmissions(string courseId, IEnumerable<Guid> slidesIds, DateTime periodStart, DateTime periodFinish)
		{
			return GetAllSubmissions(courseId, slidesIds)
				.Where(x => 
					periodStart <= x.Timestamp &&
					x.Timestamp <= periodFinish
				);
		}

		public IQueryable<UserExerciseSubmission> GetAllAcceptedSubmissions(string courseId, IEnumerable<Guid> slidesIds, DateTime periodStart, DateTime periodFinish)
		{
			return GetAllSubmissions(courseId, slidesIds, periodStart, periodFinish).Where(s => s.AutomaticCheckingIsRightAnswer);
		}

		public IQueryable<UserExerciseSubmission> GetAllAcceptedSubmissions(string courseId, IEnumerable<Guid> slidesIds)
		{
			return GetAllSubmissions(courseId, slidesIds).Where(s => s.AutomaticCheckingIsRightAnswer);
		}

		public IQueryable<UserExerciseSubmission> GetAllAcceptedSubmissionsByUser(string courseId, IEnumerable<Guid> slideIds, string userId)
		{
			return GetAllAcceptedSubmissions(courseId, slideIds).Where(s => s.UserId == userId);
		}
		
		public IQueryable<UserExerciseSubmission> GetAllAcceptedSubmissionsByUser(string courseId, Guid slideId, string userId)
		{
			return GetAllAcceptedSubmissionsByUser(courseId, new List<Guid> { slideId }, userId);
		}

		public IQueryable<UserExerciseSubmission> GetAllSubmissionsByUser(string courseId, Guid slideId, string userId)
		{
			return GetAllSubmissions(courseId, new List<Guid> { slideId }).Where(s => s.UserId == userId);
		}

		public List<AcceptedSolutionInfo> GetBestTrendingAndNewAcceptedSolutions(string courseId, List<Guid> slidesIds)
		{
			var prepared = GetAllAcceptedSubmissions(courseId, slidesIds)
				.GroupBy(x => x.CodeHash, (codeHash, ss) => new { codeHash, timestamp = ss.Min(s => s.Timestamp) })
				.Join(
					GetAllAcceptedSubmissions(courseId, slidesIds),
					g => g,
					s => new { codeHash = s.CodeHash, timestamp = s.Timestamp }, (k, s) => new { submission = s, k.timestamp })
				.Select(x => new { x.submission.Id, likes = x.submission.Likes.Count, x.timestamp })
				.ToList();

			var best = prepared
				.OrderByDescending(x => x.likes);
			var timeNow = DateTime.Now;
			var trending = prepared
				.OrderByDescending(x => (x.likes + 1) / timeNow.Subtract(x.timestamp).TotalMilliseconds);
			var newest = prepared
				.OrderByDescending(x => x.timestamp);
			var selectedSubmissionsIds = best.Take(3).Concat(trending.Take(3)).Concat(newest).Distinct().Take(10).Select(x => x.Id);

			var selectedSubmissions = db.UserExerciseSubmissions
				.Where(s => selectedSubmissionsIds.Contains(s.Id))
				.Select(s => new { s.Id, Code = s.SolutionCode.Text, Likes = s.Likes.Select(y => y.UserId) })
				.ToList();
			return selectedSubmissions
				.Select(s => new AcceptedSolutionInfo(s.Code, s.Id, s.Likes))
				.OrderByDescending(info => info.UsersWhoLike.Count)
				.ToList();
		}

		public List<AcceptedSolutionInfo> GetBestTrendingAndNewAcceptedSolutions(string courseId, Guid slideId)
		{
			return GetBestTrendingAndNewAcceptedSolutions(courseId, new List<Guid> { slideId });
		}
		
		public int GetAcceptedSolutionsCount(string courseId, Guid slideId)
		{
			return GetAllAcceptedSubmissions(courseId, new List<Guid> { slideId }).DistinctBy(x => x.UserId).Count();
		}

		public bool IsCheckingSubmissionByUser(string courseId, Guid slideId, string userId, DateTime periodStart, DateTime periodFinish)
		{
			var automaticCheckingsIds = GetAllSubmissions(courseId, new List<Guid> { slideId }, periodStart, periodFinish)
				.Where(s => s.UserId == userId)
				.Select(s => s.AutomaticCheckingId)
				.ToList();
			return db.AutomaticExerciseCheckings.Any(c => automaticCheckingsIds.Contains(c.Id) && c.Status != AutomaticExerciseCheckingStatus.Done);
		}

		public HashSet<Guid> GetIdOfPassedSlides(string courseId, string userId)
		{
			return new HashSet<Guid>(db.AutomaticExerciseCheckings
				.Where(x => x.IsRightAnswer && x.CourseId == courseId && x.UserId == userId)
				.Select(x => x.SlideId)
				.Distinct());
		}

		public IQueryable<UserExerciseSubmission> GetAllSubmissions(int max, int skip)
		{
			return db.UserExerciseSubmissions
				.OrderByDescending(x => x.Timestamp)
				.Skip(skip)
				.Take(max);
		}

		public UserExerciseSubmission FindSubmission(int id)
		{
			var submission = db.UserExerciseSubmissions.AsNoTracking().SingleOrDefault(x => x.Id == id);
			if (submission == null)
				return null;
			submission.SolutionCode = textsRepo.GetText(submission.SolutionCodeHash);
			submission.AutomaticChecking.Output = textsRepo.GetText(submission.AutomaticChecking.OutputHash);
			submission.AutomaticChecking.CompilationError = textsRepo.GetText(submission.AutomaticChecking.CompilationErrorHash);
			return submission;
		}

		public List<UserExerciseSubmission> GetUnhandledSubmissions(int count)
		{
			var notSoLongAgo = DateTime.Now - TimeSpan.FromMinutes(15);
			List<UserExerciseSubmission> submissions;
			using (var transaction = db.Database.BeginTransaction())
			{
				submissions = db.UserExerciseSubmissions
					.Where(s =>
						s.Timestamp > notSoLongAgo
						&& s.AutomaticChecking.Status == AutomaticExerciseCheckingStatus.Waiting)
					.OrderByDescending(s => s.Timestamp)
					.Take(count).ToList();
				foreach (var submission in submissions)
					submission.AutomaticChecking.Status = AutomaticExerciseCheckingStatus.Running;
				SaveAll(submissions.Select(s => s.AutomaticChecking));

				transaction.Commit();
			}
			return submissions;
		}

		public UserExerciseSubmission FindSubmissionById(int id)
		{
			return db.UserExerciseSubmissions.Find(id);
		}

		public UserExerciseSubmission FindSubmissionById(string id)
		{
			return db.UserExerciseSubmissions.Find(id);
		}

		protected List<UserExerciseSubmission> FindSubmissionsByIds(List<string> checkingsIds)
		{
			return db.UserExerciseSubmissions.Where(c => checkingsIds.Contains(c.Id.ToString())).ToList();
		}

		private void UpdateIsRightAnswerForSubmission(AutomaticExerciseChecking checking)
		{
			db.UserExerciseSubmissions
				.Where(s => s.AutomaticCheckingId == checking.Id)
				.ForEach(s => s.AutomaticCheckingIsRightAnswer = checking.IsRightAnswer);
		}

		protected void Save(AutomaticExerciseChecking checking)
		{
			db.AutomaticExerciseCheckings.AddOrUpdate(checking);
			UpdateIsRightAnswerForSubmission(checking);
			db.SaveChanges();
		}

		protected void SaveAll(IEnumerable<AutomaticExerciseChecking> checkings)
		{
			foreach (var checking in checkings)
			{
				db.AutomaticExerciseCheckings.AddOrUpdate(checking);
				UpdateIsRightAnswerForSubmission(checking);
			}
			try
			{
				db.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				throw new Exception(
					string.Join("\r\n",
					e.EntityValidationErrors.SelectMany(v => v.ValidationErrors).Select(err => err.PropertyName + " " + err.ErrorMessage)));
			}
		}

		public async Task SaveResults(RunningResults result)
		{
			var submission = FindSubmissionById(result.Id);
			var updatedChecking = await UpdateAutomaticExerciseChecking(submission.AutomaticChecking, result);
			Save(updatedChecking);
			hasHandled = true;
		}

		public async Task SaveAllResults(List<RunningResults> results)
		{
			var resultsDict = results.ToDictionary(result => result.Id);
			var submissions = FindSubmissionsByIds(results.Select(result => result.Id).ToList());
			var res = new List<AutomaticExerciseChecking>();
			foreach (var submission in submissions)
				res.Add(await UpdateAutomaticExerciseChecking(submission.AutomaticChecking, resultsDict[submission.Id.ToString()]));
			SaveAll(res);
			hasHandled = true;
		}

		private async Task<AutomaticExerciseChecking> UpdateAutomaticExerciseChecking(AutomaticExerciseChecking checking, RunningResults result)
		{
			var compilationErrorHash = (await textsRepo.AddText(result.CompilationOutput)).Hash;
			var output = result.GetOutput().NormalizeEoln();
			var outputHash = (await textsRepo.AddText(output)).Hash;

			var isWebRunner = checking.CourseId == "web" && checking.SlideId == Guid.Empty;
			var exerciseSlide = isWebRunner ? null : (ExerciseSlide)courseManager.GetCourse(checking.CourseId).GetSlideById(checking.SlideId);

			var expectedOutput = exerciseSlide?.Exercise.ExpectedOutput.NormalizeEoln();
			var isRightAnswer = result.Verdict == Verdict.Ok && output.Equals(expectedOutput);
			var score = isRightAnswer ? exerciseSlide.Exercise.CorrectnessScore : 0;

			/* For skipped slides score is always 0 */
			if (visitsRepo.IsSkipped(checking.CourseId, checking.SlideId, checking.UserId))
				score = 0;

			var newChecking = new AutomaticExerciseChecking
			{
				Id = checking.Id,
				CourseId = checking.CourseId,
				SlideId = checking.SlideId,
				UserId = checking.UserId,
				Timestamp = checking.Timestamp,

				CompilationErrorHash = compilationErrorHash,
				IsCompilationError = result.Verdict == Verdict.CompilationError,
				OutputHash = outputHash,
				ExecutionServiceName = checking.ExecutionServiceName,
				Status = AutomaticExerciseCheckingStatus.Done,
				DisplayName = checking.DisplayName,
				Elapsed = DateTime.Now - checking.Timestamp,
				IsRightAnswer = isRightAnswer,
				Score = score,
			};

			return newChecking;
		}

		public async Task<UserExerciseSubmission> RunUserSolution(
			string courseId, Guid slideId, string userId, string code,
			string compilationError, string output, bool isRightAnswer,
			string executionServiceName, string displayName, TimeSpan timeout)
		{
			var submission = await AddUserExerciseSubmission(
				courseId, slideId,
				code, isRightAnswer, compilationError, output,
				userId, executionServiceName, displayName);
			hasUnhandled = true;

			log.Info($"Запускаю проверку решения. ID посылки: {submission.Id}");

			var sw = Stopwatch.StartNew();
			while (sw.Elapsed < timeout)
			{
				await WaitHandled(TimeSpan.FromSeconds(2));
				var updatedSubmission = FindSubmission(submission.Id);
				if (updatedSubmission == null)
					return null;

				if (updatedSubmission.AutomaticChecking.Status == AutomaticExerciseCheckingStatus.Done)
				{
					log.Info($"Посылка {submission.Id} проверена. Результат: {updatedSubmission.AutomaticChecking.GetVerdict()}");
					return updatedSubmission;
				}
			}
			return null;
		}
		
		public Dictionary<int, string> GetSolutionsForSubmissions(IEnumerable<int> submissionsIds)
		{
			var solutionsHashes = db.UserExerciseSubmissions
				.Where(s => submissionsIds.Contains(s.Id))
				.Select(s => new { Hash=s.SolutionCodeHash, SubmissionId=s.Id }).ToList();
			var textsByHash = textsRepo.GetTextsByHashes(solutionsHashes.Select(s => s.Hash));
			return solutionsHashes.ToDictionary(s => s.SubmissionId, s => textsByHash.GetOrDefault(s.Hash, ""));
		}

		public async Task WaitUnhandled(TimeSpan timeout)
		{
			var sw = Stopwatch.StartNew();
			while (sw.Elapsed < timeout)
			{
				if (hasUnhandled)
				{
					hasUnhandled = false;
					return;
				}
				await Task.Delay(TimeSpan.FromMilliseconds(100));
			}
		}

		public async Task WaitHandled(TimeSpan timeout)
		{
			var sw = Stopwatch.StartNew();
			while (sw.Elapsed < timeout)
			{
				if (hasHandled)
				{
					hasHandled = false;
					return;
				}
				await Task.Delay(TimeSpan.FromMilliseconds(100));
			}
		}

		private static volatile bool hasUnhandled;
		private static volatile bool hasHandled;
	}
}