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

		public async Task<UserQuestion> AddUserQuestion(string question, string slideTitle, string userId, string userName, string unitName, DateTime time)
		{
			var userSolution = db.UserQuestions.Add(new UserQuestion
			{
				Question = question,
				UserId = userId,
				UserName = userName,
				SlideTitle = slideTitle,
				UnitName = unitName,
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

		public string GetAllQuestions(string unitName)
		{
			var answer = new StringBuilder();
			foreach (var e in db.UserQuestions)
			{
				if (e.UnitName == unitName)
					answer.Append(string.Format("{0}\n***{1}\n***{2}\n***{3}\n***{4}\n***", e.UserName, e.Time, e.UnitName, e.SlideTitle, e.Question));
			}
			return answer.ToString();
		}

	}
}