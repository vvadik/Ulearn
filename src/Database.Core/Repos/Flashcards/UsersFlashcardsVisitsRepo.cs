using System;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Repos.Flashcards
{
	public class UsersFlashcardsVisitsRepo : IUsersFlashcardsVisitsRepo
	{
		private readonly UlearnDb db;

		public UsersFlashcardsVisitsRepo(UlearnDb db)
		{
			this.db = db;
		}

		public Task AddFlashcardVisit(string userId, string courseId, Guid unitId, string flashcardId, Score score, DateTime timestamp)
		{
			throw new NotImplementedException();
		}
	}
}