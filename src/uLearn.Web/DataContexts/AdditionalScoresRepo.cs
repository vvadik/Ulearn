using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class AdditionalScoresRepo
	{
		private readonly ULearnDb db;

		public AdditionalScoresRepo() : this(new ULearnDb())
		{

		}

		public AdditionalScoresRepo(ULearnDb db)
		{
			this.db = db;
		}

		public async Task SetAdditionalScore(string courseId, Guid unitId, string userId, int score, string instructorId)
		{
			using (var transaction = db.Database.BeginTransaction())
			{
				var scores = db.AdditionalScores.Where(s => s.CourseId == courseId && s.UnitId == unitId && s.UserId == userId);
				db.AdditionalScores.RemoveRange(scores);

				var additionalScore = new AdditionalScore
				{
					CourseId = courseId,
					UnitId = unitId,
					UserId = userId,
					Score = score,
					InstructorId = instructorId,
					Timestamp = DateTime.Now,
				};
				db.AdditionalScores.Add(additionalScore);

				transaction.Commit();
				await db.SaveChangesAsync();
			}
		}

		public List<AdditionalScore> GetAdditionalScoresForUser(string courseId, string userId)
		{
			return db.AdditionalScores.Where(s => s.CourseId == courseId && s.UserId == userId).ToList();
		}

		public Dictionary<string, AdditionalScore> GetAdditionalScoresForUser(string courseId, Guid unitId, string userId)
		{
			return db.AdditionalScores
				.Where(s => s.CourseId == courseId && s.UnitId == unitId && s.UserId == userId)
				.ToDictionary(s => s.ScoringGroupId, s => s);
		}

		/* Dictionary<(userId, scoringGroupId), additionalScore> */
		public Dictionary<Tuple<string, string>, AdditionalScore> GetAdditionalScoresForUsers(string courseId, Guid unitId, IEnumerable<string> usersIds)
		{
			return db.AdditionalScores
				.Where(s => s.CourseId == courseId && s.UnitId == unitId && usersIds.Contains(s.UserId))
				.ToDictionary(s => Tuple.Create(s.UserId, s.ScoringGroupId), s => s);
		}
	}
}