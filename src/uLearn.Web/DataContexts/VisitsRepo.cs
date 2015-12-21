using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class VisitsRepo
	{
		private readonly ULearnDb db;

		public VisitsRepo() : this(new ULearnDb())
		{
			
		}

		public VisitsRepo(ULearnDb db)
		{
			this.db = db;
		}

		public async Task AddVisit(string courseId, string slideId, string userId)
		{
			if (db.Visits.Any(x => x.UserId == userId && x.SlideId == slideId))
				return;
			db.Visits.Add(new Visit
			{
				UserId = userId,
				CourseId = courseId,
				SlideId = slideId,
				Timestamp = DateTime.Now
			});
			await db.SaveChangesAsync();
		}

		public int GetVisitsCount(string slideId, string courseId)
		{
			return db.Visits.Count(x => x.SlideId == slideId);
		}

		public bool IsUserVisit(string courseId, string slideId, string userId)
		{
			return db.Visits.Any(x => x.SlideId == slideId && x.UserId == userId);
		}

		public HashSet<string> GetIdOfVisitedSlides(string courseId, string userId)
		{
			return new HashSet<string>(db.Visits.Where(x => x.UserId == userId && x.CourseId == courseId).Select(x => x.SlideId));
		}
		
		public bool HasVisitedSlides(string courseId, string userId)
		{
			return db.Visits.Any(x => x.UserId == userId && x.CourseId == courseId);
		}

		private async Task UpdateAttempts(string slideId, string userId, Action<Visit> action)
		{
			var visit = db.Visits.FirstOrDefault(v => v.SlideId == slideId && v.UserId == userId);
			if (visit == null)
				return;
			action(visit);
			await db.SaveChangesAsync();
		}

		public async Task RemoveAttempts(string slideId, string userId)
		{
			await UpdateAttempts(slideId, userId, visit =>
			{
				visit.AttemptsCount = 0;
				visit.Score = 0;
				visit.IsPassed = false;
			});
		}

		public async Task AddAttempt(string slideId, string userId, int score)
		{
			await UpdateAttempts(slideId, userId, visit =>
			{
				visit.AttemptsCount++;
				visit.Score = score;
				visit.IsPassed = true;
			});
		}

		public async Task DropAttempt(string slideId, string userId)
		{
			await UpdateAttempts(slideId, userId, visit =>
			{
				visit.Score = 0;
				visit.IsPassed = false;
			});
		}

		public async Task AddSolutionAttempt(string slideId, string userId, bool isRightAnswer)
		{
			await UpdateAttempts(slideId, userId, visit =>
			{
				visit.IsPassed = visit.IsPassed || isRightAnswer;
				visit.AttemptsCount++;
				var newScore = isRightAnswer && !visit.IsSkipped ? 5 : 0;
				if (newScore > visit.Score)
					visit.Score = newScore;
			});
		}

		public Dictionary<string, int> GetScoresForSlides(string courseId, string userId)
		{
			return db.Visits
				.Where(v => v.CourseId == courseId && v.UserId == userId)
				.GroupBy(v => v.SlideId, (s, visits) => new { Key = s, Value = visits.FirstOrDefault()})
				.ToDictionary(g => g.Key, g => g.Value.Score);
		}

		public int GetScore(string slideId, string userId)
		{
			return db.Visits
				.Where(v => v.SlideId == slideId && v.UserId == userId)
				.Select(v => v.Score)
				.FirstOrDefault();
		}

		public Visit GetVisiter(string slideId, string userId)
		{
			return db.Visits.FirstOrDefault(v => v.SlideId == slideId && v.UserId == userId);
		}

		public async Task SkipSlide(string courseId, string slideId, string userId)
		{
			var visiter = db.Visits.FirstOrDefault(v => v.SlideId == slideId && v.UserId == userId);
			if (visiter != null)
				visiter.IsSkipped = true;
			else
				db.Visits.Add(new Visit
				{
					UserId = userId,
					CourseId = courseId,
					SlideId = slideId,
					Timestamp = DateTime.Now,
					IsSkipped = true
				});
			await db.SaveChangesAsync();
		}

		public bool IsSkipped(string slideId, string userId)
		{
			return db.Visits.Any(v => v.SlideId == slideId && v.UserId == userId && v.IsSkipped);
		}

		public bool IsPassed(string slideId, string userId)
		{
			return db.Visits.Any(v => v.SlideId == slideId && v.UserId == userId && v.IsPassed);
		}

		public bool IsSkippedOrPassed(string slideId, string userId)
		{
			return db.Visits.Any(v => v.SlideId == slideId && v.UserId == userId && (v.IsPassed || v.IsSkipped));
		}

		public async Task AddVisits(IEnumerable<Visit> visits)
		{
			foreach (var visit in visits)
			{
				if (db.Visits.Any(x => x.UserId == visit.UserId && x.SlideId == visit.SlideId))
					continue;
				db.Visits.Add(visit);
			}
			await db.SaveChangesAsync();
		}
	}
}