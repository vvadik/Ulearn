using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Database.Repos
{
	/* TODO (andgein): This repo is not fully migrated to .NET Core and EF Core */
	public class AdditionalScoresRepo : IAdditionalScoresRepo
	{
		private readonly UlearnDb db;
		private readonly ILogger logger;

		public AdditionalScoresRepo(UlearnDb db, ILogger logger)
		{
			this.db = db;
			this.logger = logger;
		}

		public async Task<AdditionalScore> SetAdditionalScore(string courseId, Guid unitId, string userId, string scoringGroupId, int score, string instructorId)
		{
			using (var transaction = db.Database.BeginTransaction())
			{
				var scores = db.AdditionalScores.Where(s => s.CourseId == courseId && s.UnitId == unitId && s.UserId == userId && s.ScoringGroupId == scoringGroupId);
				db.AdditionalScores.RemoveRange(scores);

				var additionalScore = new AdditionalScore
				{
					CourseId = courseId,
					UnitId = unitId,
					UserId = userId,
					ScoringGroupId = scoringGroupId,
					Score = score,
					InstructorId = instructorId,
					Timestamp = DateTime.Now,
				};
				db.AdditionalScores.Add(additionalScore);

				transaction.Commit();
				await db.SaveChangesAsync();

				return additionalScore;
			}
		}

		/* Dictionary<(unitId, scoringGroupId), additionalScore> */

		public async Task<Dictionary<Tuple<Guid, string>, int>> GetAdditionalScoresForUser(string courseId, string userId)
		{
			return (await db.AdditionalScores
				.Where(s => s.CourseId == courseId && s.UserId == userId)
				.ToListAsync().ConfigureAwait(false))
				.ToDictSafe(s => Tuple.Create(s.UnitId, s.ScoringGroupId), s => s.Score);
		}

		public Dictionary<string, AdditionalScore> GetAdditionalScoresForUser(string courseId, Guid unitId, string userId)
		{
			return db.AdditionalScores
				.Where(s => s.CourseId == courseId && s.UnitId == unitId && s.UserId == userId)
				.ToList()
				.ToDictSafe(s => s.ScoringGroupId, s => s);
		}

		/* Dictionary<(userId, scoringGroupId), additionalScore> */
		public Dictionary<Tuple<string, string>, AdditionalScore> GetAdditionalScoresForUsers(string courseId, Guid unitId, IEnumerable<string> usersIds)
		{
			return db.AdditionalScores
				.Where(s => s.CourseId == courseId && s.UnitId == unitId && usersIds.Contains(s.UserId))
				.ToList()
				.ToDictSafe(s => Tuple.Create(s.UserId, s.ScoringGroupId), s => s);
		}

		/* Dictionary<(userId, unitId, scoringGroupId), additionalScore> */
		public async Task<Dictionary<Tuple<string, Guid, string>, AdditionalScore>> GetAdditionalScoresForUsers(string courseId, IEnumerable<string> usersIds)
		{
			return (await db.AdditionalScores
				.Where(s => s.CourseId == courseId && usersIds.Contains(s.UserId))
				.ToListAsync().ConfigureAwait(false))
				.ToDictSafe(s => Tuple.Create(s.UserId, s.UnitId, s.ScoringGroupId), s => s);
		}

		public async Task RemoveAdditionalScores(string courseId, Guid unitId, string userId, string scoringGroupId)
		{
			var scores = db.AdditionalScores.Where(s => s.CourseId == courseId && s.UnitId == unitId && s.UserId == userId && s.ScoringGroupId == scoringGroupId);
			db.AdditionalScores.RemoveRange(scores);
			await db.SaveChangesAsync();
		}
	}
}