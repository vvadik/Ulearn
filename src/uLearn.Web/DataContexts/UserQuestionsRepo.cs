using System;
using System.Threading.Tasks;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class UserQuestionsRepo
	{
		private readonly ULearnDb db;

		public UserQuestionsRepo(ULearnDb db)
		{
			this.db = db;
		}

		public void Delete(UserQuestion userSolution)
		{
			db.UserQuestions.Remove(userSolution);
			db.SaveChanges();
		}
	}
}