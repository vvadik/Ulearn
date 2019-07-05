using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repos.Flashcards
{
	public class UsersFlashcardsVisitsRepo : IUsersFlashcardsVisitsRepo
	{
		private readonly UlearnDb db;

		public UsersFlashcardsVisitsRepo(UlearnDb db)
		{
			this.db = db;
		}

		public async Task AddFlashcardVisitAsync(string userId, string courseId, Guid unitId, string flashcardId, Score score, DateTime timestamp)
		{
			var existingRecords = await db.UserFlashcardsVisits
				.Where(c => c.UserId == userId && c.CourseId == courseId && c.UnitId == unitId && c.FlashcardId == flashcardId)
				.ToListAsync();
			if (existingRecords.Count != 0)
			{
				db.UserFlashcardsVisits.RemoveRange(existingRecords);
			}

			db.UserFlashcardsVisits.Add(new UserFlashcardsVisit 
				{ UserId = userId, CourseId = courseId, UnitId = unitId, FlashcardId = flashcardId, Score = score, Timestamp = timestamp });
			await db.SaveChangesAsync();
		}

		public async Task<List<UserFlashcardsVisit>> GetUserFlashcardsVisitsAsync(string userId)
		{
			return await db.UserFlashcardsVisits.Where(c => c.UserId == userId).ToListAsync();
		}

		public async Task<List<UserFlashcardsVisit>> GetUserFlashcardsVisitsAsync(string userId, Guid unitId)
		{
			return await db.UserFlashcardsVisits.Where(c => c.UserId == userId && c.UnitId == unitId).ToListAsync();
		}

		public async Task<List<UserFlashcardsVisit>> GetUserFlashcardsVisitsAsync(string userId, string courseId)
		{
			return await db.UserFlashcardsVisits.Where(c => c.UserId == userId && c.CourseId == courseId).ToListAsync();
		}
	}
}