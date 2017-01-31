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

		public void GetAdditionalScoresForUser(string courseId, string userId)
		{
			var scores = db.AdditionalScores.Where(s => s.CourseId == courseId && s.UserId == userId);
		}
	}
}