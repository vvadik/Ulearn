using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.SqlServer.Server;
using uLearn.Web.Models;
using uLearn;

namespace uLearn.Web.DataContexts
{
	public class UserQuestionsRepo
	{
		private readonly ULearnDb db;

		public UserQuestionsRepo() : this(new ULearnDb())
		{

		}

		public UserQuestionsRepo(ULearnDb db)
		{
			this.db = db;
		}

		public async Task<UserQuestion> AddUserQuestion(string question, string courseId, Slide slide, string userId, string userName, DateTime time)
		{
			var userSolution = db.UserQuestions.Add(new UserQuestion
			{
				Question = question,
				UserId = userId,
				UserName = userName,
				SlideTitle = slide.Title,
				UnitName = slide.Info.UnitName,
				SlideId = slide.Id,
				CourseId = courseId,
				Time = time
			});
			await db.SaveChangesAsync();
			return userSolution;
		}

		public void Delete(UserQuestion userSolution)
		{
			db.UserQuestions.Remove(userSolution);
			db.SaveChanges();
		}
	}
}