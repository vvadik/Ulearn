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
				LikersStorage = new List<Like>()
			});
			await db.SaveChangesAsync();
			return userSolution;
		}

		public void Delete(UserSolution userSolution)
		{
			db.UserSolutions.Remove(userSolution);
			db.SaveChanges();
		}

		public async Task<Like> Like(int id, string userId)
		{
			var solutionForLike = db.UserSolutions.Find(id);
			solutionForLike.LikersStorage.Add(new Like {SolutionId = id, Timestamp = DateTime.Now, UserId = userId});
			await db.SaveChangesAsync();
			return new Like {SolutionId = id, Timestamp = DateTime.Now, UserId = userId};
		}

		public List<AcceptedSolutionInfo> GetAllAcceptedSolutions(int slideIndex)
		{
			var timeNow = DateTime.Now;
			var stringSlideIndex = slideIndex.ToString();
			var prepared = db.UserSolutions
				.Where(x => x.IsRightAnswer && x.SlideId == stringSlideIndex)
				.ToList();
			var answer = prepared
				.OrderByDescending(x => x.LikersStorage.Count /*/ (timeNow - x.Timestamp).TotalSeconds*/)
				.Take(10).Select(x => new AcceptedSolutionInfo(x.Code, x.Id, x.LikersStorage.Select(y => y.UserId))).ToList();
			return answer;
		}

		public bool IsUserPassedTask(string courseId, int slideIndex, string userId)
		{
			var slideId = slideIndex.ToString();
			return db.UserSolutions.Any(x => x.SlideId == slideId && x.CourseId == courseId && x.UserId == userId && x.IsRightAnswer);
		}
	}
}