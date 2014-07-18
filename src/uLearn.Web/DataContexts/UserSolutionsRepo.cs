using System;
using System.Collections.Generic;
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

		public async Task<UserSolution> AddUserSolution(string courseId, int slideIndex, string code, bool isRightAnswer,
			string compilationError,
			string output, string userId)
		{
			var userSolution = db.UserSolutions.Add(new UserSolution
			{
				Code = code,
				CompilationError = compilationError,
				CourseId = courseId,
				SlideId = slideIndex.ToString(),
				IsCompilationError = !string.IsNullOrWhiteSpace(compilationError),
				IsRightAnswer = isRightAnswer,
				Output = output,
				Timestamp = DateTime.Now,
				UserId = userId,
				CodeHash = code.GetHashCode(),
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

		public async Task<string> Like(int solutionId, string userId)
		{
			var solutionForLike = db.UserSolutions.Find(solutionId);
			if (solutionForLike.Likes.Any(like => like.UserId == userId))
			{
				return "already been";
			}
			solutionForLike.Likes.Add(new Like {SolutionId = solutionId, Timestamp = DateTime.Now, UserId = userId});
			await db.SaveChangesAsync();
			return "success";
		}

		public List<AcceptedSolutionInfo> GetAllAcceptedSolutions(string courseId, int slideIndex)
		{
			var timeNow = DateTime.Now;
			var stringSlideIndex = slideIndex.ToString();
			var prepared = db.UserSolutions
				.Where(x => x.IsRightAnswer && x.SlideId == stringSlideIndex && x.CourseId == courseId)
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

		public bool IsUserPassedTask(string courseId, int slideIndex, string userId)
		{
			var slideId = slideIndex.ToString();
			return db.UserSolutions.Any(x => x.SlideId == slideId && x.CourseId == courseId && x.UserId == userId && x.IsRightAnswer);
		}

		public string GetLatestAcceptedSolution(string courseId, int slideIndex, string userId)
		{
			var timeNow = DateTime.Now;
			var slideId = slideIndex.ToString();
			var allUserSolutionOnThisTask = db.UserSolutions
				.Where(x => x.SlideId == slideId && x.CourseId == courseId && x.UserId == userId && x.IsRightAnswer).ToList();
			var answer = allUserSolutionOnThisTask
				.OrderBy(x => timeNow.Subtract(x.Timestamp).TotalMilliseconds)
				.First()
				.Code;
			return answer;
		}
	}
}