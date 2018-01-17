using Database.Models;

namespace Database.DataContexts
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