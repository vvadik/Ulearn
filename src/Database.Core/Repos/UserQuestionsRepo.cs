using Database.Models;

namespace Database.Repos
{
	public class UserQuestionsRepo
	{
		private readonly UlearnDb db;

		public UserQuestionsRepo(UlearnDb db)
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