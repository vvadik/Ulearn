using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using NUnit.Framework;
using RunCsJob;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class UnitsRepo
	{
		private readonly ULearnDb db;
		private readonly CourseManager courseManager;

		public UnitsRepo() : this(new ULearnDb(), WebCourseManager.Instance)
		{

		}

		public UnitsRepo(ULearnDb db, CourseManager courseManager)
		{
			this.db = db;
			this.courseManager = courseManager;
		}

		public List<string> GetVisibleUnits(string courseId, IPrincipal user)
		{
			var canSeeEverything = user.IsInRole(LmsRoles.Tester) || user.IsInRole(LmsRoles.Admin) || user.IsInRole(LmsRoles.Instructor);
			if (canSeeEverything)
				return courseManager.GetCourse(courseId).Slides.Select(s => s.Info.UnitName).Distinct().ToList();
			return db.Units.Where(u => u.CourseId == courseId && u.PublishTime <= DateTime.Now).Select(u => u.UnitName).ToList();
		}

		public DateTime GetNextUnitPublishTime(string courseId)
		{
			return db.Units.Where(u => u.CourseId == courseId && u.PublishTime > DateTime.Now).Select(u => u.PublishTime).Concat(new[] { DateTime.MaxValue }).Min();
		}
	}

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

		public async Task<UserSolution> AddUserSolution(string courseId, string slideId, string code, bool isRightAnswer, string compilationError, string output, string userId, string executionServiceName, string displayName)
		{
			if (string.IsNullOrWhiteSpace(code))
				code = "// no code";
			var hash = (await textsRepo.AddText(code)).Hash;
			var compilationErrorHash = (await textsRepo.AddText(compilationError)).Hash;
			var outputHash = (await textsRepo.AddText(output)).Hash;

			var userSolution = db.UserSolutions.Add(new UserSolution
			{
				SolutionCodeHash = hash,
				CompilationErrorHash = compilationErrorHash,
				CourseId = courseId,
				SlideId = slideId,
				IsCompilationError = !string.IsNullOrWhiteSpace(compilationError),
				IsRightAnswer = isRightAnswer,
				OutputHash = outputHash,
				Timestamp = DateTime.Now,
				UserId = userId,
				CodeHash = code.Split('\n').Select(x => x.Trim()).Aggregate("", (x, y) => x + y).GetHashCode(),
				Likes = new List<Like>(),
				ExecutionServiceName = executionServiceName,
				DisplayName = displayName,
				Status = SubmissionStatus.Waiting
			});
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
			return userSolution;
		}

		public void Delete(UserSolution userSolution)
		{
			db.UserSolutions.Remove(userSolution);
			db.SaveChanges();
		}

		///<returns>(likesCount, isLikedByThisUsed)</returns>
		public async Task<Tuple<int, bool>> Like(int solutionId, string userId)
		{
			var solutionForLike = db.UserSolutions.Find(solutionId);
			if (solutionForLike == null) throw new Exception("Solution " + solutionId + " not found");
			var hisLike = db.SolutionLikes.FirstOrDefault(like => like.UserId == userId && like.UserSolutionId == solutionId);
			var votedAlready = hisLike != null;
			var likesCount = solutionForLike.Likes.Count();
			if (votedAlready)
			{
				db.SolutionLikes.Remove(hisLike);
				likesCount--;
			}
			else
			{
				db.SolutionLikes.Add(new Like {UserSolutionId = solutionId, Timestamp = DateTime.Now, UserId = userId});
				likesCount++;
			}
			await db.SaveChangesAsync();
			return Tuple.Create(likesCount, !votedAlready);
		}

		public List<AcceptedSolutionInfo> GetAllAcceptedSolutions(string courseId, string slideId)
		{
			var prepared = db.UserSolutions
				.Where(x => x.IsRightAnswer && x.SlideId == slideId)
				.GroupBy(x => x.CodeHash, (codeHash, ss) => new { codeHash, timestamp = ss.Min(s => s.Timestamp) })
				.Join(
					db.UserSolutions.Where(x => x.IsRightAnswer && x.SlideId == slideId), 
					g => g, 
					s => new { codeHash = s.CodeHash, timestamp = s.Timestamp }, (k, s) => new {sol = s, k.timestamp})
				.Select(x => new { x.sol.Id, likes = x.sol.Likes.Count, x.timestamp })
				.ToList();
			
			var best = prepared
				.OrderByDescending(x => x.likes);
			var timeNow = DateTime.Now;
			var trending = prepared
				.OrderByDescending(x => (x.likes + 1) / timeNow.Subtract(x.timestamp).TotalMilliseconds);
			var newest = prepared
				.OrderByDescending(x => x.timestamp);
			var answer = best.Take(3).Concat(trending.Take(3)).Concat(newest).Distinct().Take(10).Select(x => x.Id);
			var result = db.UserSolutions
				.Where(solution => answer.Contains(solution.Id))
				.Select(solution => new { solution.Id, Code = solution.SolutionCode.Text,  Likes = solution.Likes.Select(y => y.UserId)})
				.ToList();
			return result
				.Select(x => new AcceptedSolutionInfo(x.Code, x.Id, x.Likes))
				.OrderByDescending(info => info.UsersWhoLike.Count)
				.ToList();
		}

		public string FindLatestAcceptedSolution(string courseId, string slideId, string userId)
		{
			var allUserSolutionOnThisTask = db.UserSolutions
				.Where(x => x.SlideId == slideId && x.UserId == userId && x.IsRightAnswer).ToList();
			var answer = allUserSolutionOnThisTask
				.OrderByDescending(x => x.Timestamp)
				.FirstOrDefault();
			return answer == null ? null : answer.SolutionCode.Text;
		}

		public int GetAcceptedSolutionsCount(string slideId, string courseId)
		{
			return db.UserSolutions.Where(x => x.SlideId == slideId && x.IsRightAnswer).Select(x => x.UserId).Distinct().Count();
		}

		public HashSet<string> GetIdOfPassedSlides(string courseId, string userId)
		{
			return new HashSet<string>(db.UserSolutions
				.Where(x => x.IsRightAnswer && x.CourseId == courseId && x.UserId == userId)
				.Select(x => x.SlideId)
				.Distinct());
		}

		public IEnumerable<UserSolution> GetAllSolutions(int max, int skip)
		{
			return db.UserSolutions
				.OrderByDescending(x => x.Timestamp)
				.Skip(skip)
				.Take(max);
		}

		public UserSolution GetDetails(int id)
		{
			var solution = db.UserSolutions.AsNoTracking().SingleOrDefault(x => x.Id == id);
			if (solution == null)
				return null;
			solution.SolutionCode = textsRepo.GetText(solution.SolutionCodeHash);
			solution.Output = textsRepo.GetText(solution.OutputHash);
			solution.CompilationError = textsRepo.GetText(solution.CompilationErrorHash);
			return solution;
		}

		public List<UserSolution> GetUnhandled(int count)
		{
			var hourAgo = DateTime.Now - TimeSpan.FromHours(1);
			var result = db.UserSolutions
				.Where(s =>
					s.Timestamp > hourAgo
					&& s.Status == SubmissionStatus.Waiting)
				.Take(count).ToList();
			foreach (var details in result)
				details.Status = SubmissionStatus.Running;
			SaveAll(result);
			return result;
		}

		protected UserSolution Find(string id)
		{
			return db.UserSolutions.Find(id);
		}

		protected List<UserSolution> FindAll(List<string> submissions)
		{
			return db.UserSolutions.Where(details => submissions.Contains(details.Id.ToString())).ToList();
		}

		protected void Save(UserSolution solution)
		{
			db.UserSolutions.AddOrUpdate(solution);
			db.SaveChanges();
		}

		protected void SaveAll(IEnumerable<UserSolution> items)
		{
			foreach (var details in items)
				db.UserSolutions.AddOrUpdate(details);
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
			var solution = Find(result.Id);
			var updatedSolution = await UpdateSubmission(solution, result);
			Save(updatedSolution);
			hasHandled = true;
		}

		public async Task SaveAllResults(List<RunningResults> results)
		{
			var resultsDict = results.ToDictionary(result => result.Id);
			var submissions = FindAll(results.Select(result => result.Id).ToList());
			var res = new List<UserSolution>();
			foreach (var submission in submissions)
				res.Add(await UpdateSubmission(submission, resultsDict[submission.Id.ToString()]));
			SaveAll(res);
			hasHandled = true;
		}

		private async Task<UserSolution> UpdateSubmission(UserSolution submission, RunningResults result)
		{
			var compilationErrorHash = (await textsRepo.AddText(result.CompilationOutput)).Hash;
			var outputHash = (await textsRepo.AddText(result.GetOutput().NormalizeEoln())).Hash;

			var webRunner = submission.CourseId == "web" && submission.SlideId == "runner";
			var exerciseSlide = webRunner ? null : ((ExerciseSlide)courseManager.GetCourse(submission.CourseId).GetSlideById(submission.SlideId));
			var updated = new UserSolution
			{
				Id = submission.Id,
				SolutionCodeHash = submission.SolutionCodeHash,
				CompilationErrorHash = compilationErrorHash,
				CourseId = submission.CourseId,
				SlideId = submission.SlideId,
				IsCompilationError = result.Verdict == Verdict.CompilationError,
				IsRightAnswer = result.Verdict == Verdict.Ok 
					&& (webRunner || exerciseSlide.Exercise.ExpectedOutput.NormalizeEoln() == result.GetOutput().NormalizeEoln()),
				OutputHash = outputHash,
				Timestamp = submission.Timestamp,
				UserId = submission.UserId,
				CodeHash = submission.CodeHash,
				Likes = submission.Likes,
				ExecutionServiceName = submission.ExecutionServiceName,
				Status = SubmissionStatus.Done,
				DisplayName = submission.DisplayName,
				Elapsed = DateTime.Now - submission.Timestamp
			};

			return updated;
		}

		public async Task<UserSolution> RunUserSolution(
			string courseId, string slideId, string userId, string code, 
			string compilationError, string output, bool isRightAnswer, 
			string executionServiceName, string displayName, TimeSpan timeout)
		{
			var solution = await AddUserSolution(
				courseId, slideId,
				code, isRightAnswer, compilationError, output,
				userId, executionServiceName, displayName);
			hasUnhandled = true;

			var sw = Stopwatch.StartNew();
			while (sw.Elapsed < timeout)
			{
				await WaitHandled(TimeSpan.FromSeconds(2));
				var details = GetDetails(solution.Id);
				if (details.Status == SubmissionStatus.Done)
					return details;
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