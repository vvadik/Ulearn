using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Models;
using Ulearn.Core.Courses.Flashcards;

namespace Database.Repos.Flashcards
{
	public interface IUsersFlashcardsVisitsRepo
	{
		Task AddFlashcardVisitAsync(string userId, string courseId, Guid unitId, string flashcardId, Score score, DateTime timestamp);
		Task<List<UserFlashcardsVisit>> GetUserFlashcardsVisitsAsync(string userId,string courseId ,Guid unitId);
		Task<List<UserFlashcardsVisit>> GetUserFlashcardsVisitsAsync(string userId, string courseId);
	}
}