using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.SqlServer.Server;
using uLearn.Web.Models;
using uLearn;

namespace uLearn.Web.DataContexts
{
	public class UserSolutionsRepo
	{
		private readonly ULearnDb db;

		public UserSolutionsRepo() : this(new ULearnDb())
		{
			
		}

		public UserSolutionsRepo(ULearnDb db)
		{
			this.db = db;
		}

		public async Task<UserSolution> AddUserSolution(string courseId, string slideId, string code, bool isRightAnswer,
			string compilationError,
			string output, string userId)
		{
			var userSolution = db.UserSolutions.Add(new UserSolution
			{
				Code = code,
				CompilationError = compilationError,
				CourseId = courseId,
				SlideId = slideId,
				IsCompilationError = !string.IsNullOrWhiteSpace(compilationError),
				IsRightAnswer = isRightAnswer,
				Output = output,
				Timestamp = DateTime.Now,
				UserId = userId,
				CodeHash = code.Split('\n').Select(x => x.Trim()).Aggregate("", (x, y) => x + y).GetHashCode(),
				Likes = new List<Like>()
			});
			await db.SaveChangesAsync();
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
			var hisLike = solutionForLike.Likes.FirstOrDefault(like => like.UserId == userId);
			var votedAlready = hisLike != null;
			if (votedAlready)
				solutionForLike.Likes.Remove(hisLike);
			else
				solutionForLike.Likes.Add(new Like {SolutionId = solutionId, Timestamp = DateTime.Now, UserId = userId});
			await db.SaveChangesAsync();
			return Tuple.Create(solutionForLike.Likes.Count(), !votedAlready);
		}

		public List<AcceptedSolutionInfo> GetAllAcceptedSolutions(string courseId, string slideId)
		{
			var timeNow = DateTime.Now;
			var prepared = db.UserSolutions
				.Where(x => x.IsRightAnswer && x.SlideId == slideId && x.CourseId == courseId)
				.ToList();
			var answer = prepared
				.GroupBy(x => x.CodeHash)
				.Select(x => x.OrderByDescending(y => timeNow.Subtract(y.Timestamp).TotalMilliseconds))
				.Select(x => x.First())
				.OrderByDescending(x => (x.Likes.Count+1)/timeNow.Subtract(x.Timestamp).TotalMilliseconds)
				.Take(10)
				.Select(x => new AcceptedSolutionInfo(x.Code, x.Id, x.Likes.Select(y => y.UserId)))
				.ToList();
			return answer;
		}

		public bool IsUserPassedTask(string courseId, string slideId, string userId)
		{
			return db.UserSolutions.Any(x => x.SlideId == slideId && x.CourseId == courseId && x.UserId == userId && x.IsRightAnswer);
		}

		public string GetLatestAcceptedSolution(string courseId, string slideId, string userId)
		{
			var timeNow = DateTime.Now;
			var allUserSolutionOnThisTask = db.UserSolutions
				.Where(x => x.SlideId == slideId && x.CourseId == courseId && x.UserId == userId && x.IsRightAnswer).ToList();
			var answer = allUserSolutionOnThisTask
				.OrderBy(x => timeNow.Subtract(x.Timestamp).TotalMilliseconds)
				.First()
				.Code;
			return answer;
		}

		public int GetAcceptedSolutionsCount(string slideId, string courseId)
		{
			return db.UserSolutions.Where(x => x.SlideId == slideId && x.CourseId == courseId && x.IsRightAnswer).Select(x => x.UserId).Distinct().Count();
		}
	}
}