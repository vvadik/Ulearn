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

		public async Task AddFlashcardVisitAsync(string userId, string courseId, Guid unitId, string flashcardId, Rate rate, DateTime timestamp)
		{
			courseId = courseId.ToLower();
			var existingRecord = db.UserFlashcardsVisits.FirstOrDefault(c => c.UserId == userId && c.CourseId == courseId && c.UnitId == unitId && c.FlashcardId == flashcardId);

			if (existingRecord != null)
			{
				existingRecord.Score = rate;
				existingRecord.Timestamp = timestamp;
			}
			else
			{
				db.UserFlashcardsVisits.Add(new UserFlashcardsVisit
					{ UserId = userId, CourseId = courseId, UnitId = unitId, FlashcardId = flashcardId, Score = rate, Timestamp = timestamp });
			}

			await db.SaveChangesAsync();
		}

		public async Task<List<UserFlashcardsVisit>> GetUserFlashcardsVisitsAsync(string userId, string courseId, Guid unitId)
		{
			courseId = courseId.ToLower();
			return await db.UserFlashcardsVisits.Where(c => c.UserId == userId && c.CourseId == courseId && c.UnitId == unitId).ToListAsync();
		}

		public async Task<List<UserFlashcardsVisit>> GetUserFlashcardsVisitsAsync(string userId, string courseId)
		{
			courseId = courseId.ToLower();
			return await db.UserFlashcardsVisits.Where(c => c.UserId == userId && c.CourseId == courseId).ToListAsync();
		}
	}
}