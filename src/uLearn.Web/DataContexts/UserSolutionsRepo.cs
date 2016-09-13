using RunCsJob.Api;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class UserSolutionsRepo
	{
		private readonly ULearnDb db;
		private readonly TextsRepo textsRepo = new TextsRepo();
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
			};
			
			db.UserExerciseSubmissions.Add(submission);

			try
			{
				await db.SaveChangesAsync();
			}
			catch (DbEntityValidationException e)
			{
				throw new Exception(
					string.Join("\r\n",
						e.EntityValidationErrors.SelectMany(v => v.ValidationErrors).Select(err => err.PropertyName + " " + err.ErrorMessage)));
			}

			return submission;
		}

		public void DeleteSubmission(UserExerciseSubmission submission)
		{
			db.AutomaticExerciseCheckings.Remove(submission.AutomaticChecking);
			db.ManualExerciseCheckings.RemoveRange(submission.ManualCheckings);
			db.UserExerciseSubmissions.Remove(submission);
			db.SaveChanges();
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

		public IQueryable<UserExerciseSubmission> GetAllSubmissions(string courseId, IEnumerable<Guid> slidesIds, DateTime periodStart, DateTime periodFinish)
		{
			return db.UserExerciseSubmissions
				.Include(s => s.AutomaticChecking)
				.Where(
					x => x.CourseId == courseId &&
					slidesIds.Contains(x.SlideId) &&
					periodStart <= x.Timestamp &&
					x.Timestamp <= periodFinish
				);
		}

		public IEnumerable<UserExerciseSubmission> GetAllAcceptedSubmissions(string courseId, IEnumerable<Guid> slidesIds, DateTime periodStart, DateTime periodFinish)
		{
			return GetAllSubmissions(courseId, slidesIds, periodStart, periodFinish).Where(s => s.AutomaticChecking.IsRightAnswer);
		}

		public List<AcceptedSolutionInfo> GetBestTrendingAndNewAcceptedSolutions(string courseId, IEnumerable<Guid> slidesIds)
		{
			var prepared = db.UserExerciseSubmissions
				.Where(x => x.AutomaticChecking.IsRightAnswer && slidesIds.Contains(x.SlideId))
				.GroupBy(x => x.CodeHash, (codeHash, ss) => new { codeHash, timestamp = ss.Min(s => s.Timestamp) })
				.Join(
					db.UserExerciseSubmissions.Where(x => x.AutomaticChecking.IsRightAnswer && slidesIds.Contains(x.SlideId)),
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
			var answer = best.Take(3).Concat(trending.Take(3)).Concat(newest).Distinct().Take(10).Select(x => x.Id);
			var result = db.UserExerciseSubmissions
				.Where(solution => answer.Contains(solution.Id))
				.Select(solution => new { solution.Id, Code = solution.SolutionCode.Text, Likes = solution.Likes.Select(y => y.UserId) })
				.ToList();
			return result
				.Select(x => new AcceptedSolutionInfo(x.Code, x.Id, x.Likes))
				.OrderByDescending(info => info.UsersWhoLike.Count)
				.ToList();
		}

		public List<AcceptedSolutionInfo> GetBestTrendingAndNewAcceptedSolutions(string courseId, Guid slideId)
		{
			return GetBestTrendingAndNewAcceptedSolutions(courseId, new List<Guid> { slideId });
		}

		public UserExerciseSubmission FindLatestAcceptedSubmission(string courseId, Guid slideId, string userId)
		{
			var allUserSolutionOnThisTask = db.UserExerciseSubmissions
				.Where(x => x.CourseId == courseId && x.SlideId == slideId && x.UserId == userId && x.AutomaticChecking.IsRightAnswer);
			var latest = allUserSolutionOnThisTask
				.OrderByDescending(x => x.Timestamp)
				.FirstOrDefault();
			return latest;
		}
		
		public int GetAcceptedSolutionsCount(Guid slideId, string courseId)
		{
			return db.AutomaticExerciseCheckings.Where(x => x.SlideId == slideId && x.IsRightAnswer).Select(x => x.UserId).Distinct().Count();
		}

		public HashSet<Guid> GetIdOfPassedSlides(string courseId, string userId)
		{
			return new HashSet<Guid>(db.AutomaticExerciseCheckings
				.Where(x => x.IsRightAnswer && x.CourseId == courseId && x.UserId == userId)
				.Select(x => x.SlideId)
				.Distinct());
		}

		public IEnumerable<UserExerciseSubmission> GetAllSubmissions(int max, int skip)
		{
			return db.UserExerciseSubmissions
				.OrderByDescending(x => x.Timestamp)
				.Skip(skip)
				.Take(max);
		}

		public UserExerciseSubmission GetSubmission(int id)
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
			var submissions = db.UserExerciseSubmissions
				.Where(s =>
					s.Timestamp > notSoLongAgo
					&& s.AutomaticChecking.Status == AutomaticExerciseCheckingStatus.Waiting)
				.OrderByDescending(s => s.Timestamp)
				.Take(count).ToList();
			foreach (var submission in submissions)
				submission.AutomaticChecking.Status = AutomaticExerciseCheckingStatus.Running;
			SaveAll(submissions.Select(s => s.AutomaticChecking));
			return submissions;
		}

		protected UserExerciseSubmission FindSubmissionById(string id)
		{
			return db.UserExerciseSubmissions.Find(id);
		}

		protected List<UserExerciseSubmission> FindSubmissionsByIds(List<string> checkingsIds)
		{
			return db.UserExerciseSubmissions.Where(c => checkingsIds.Contains(c.Id.ToString())).ToList();
		}

		protected void Save(AutomaticExerciseChecking checking)
		{
			db.AutomaticExerciseCheckings.AddOrUpdate(checking);
			db.SaveChanges();
		}

		protected void SaveAll(IEnumerable<AutomaticExerciseChecking> checkings)
		{
			foreach (var checking in checkings)
				db.AutomaticExerciseCheckings.AddOrUpdate(checking);
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

			var sw = Stopwatch.StartNew();
			while (sw.Elapsed < timeout)
			{
				await WaitHandled(TimeSpan.FromSeconds(2));
				var updatedSubmission = GetSubmission(submission.Id);
				if (updatedSubmission.AutomaticChecking.Status == AutomaticExerciseCheckingStatus.Done)
					return updatedSubmission;
			}
			return null;
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