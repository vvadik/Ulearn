using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

		public async Task AddVisit(string courseId, Guid slideId, string userId)
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

		public int GetVisitsCount(Guid slideId, string courseId)
		{
			return db.Visits.Count(x => x.SlideId == slideId);
		}

		public bool IsUserVisit(string courseId, Guid slideId, string userId)
		{
			return db.Visits.Any(x => x.SlideId == slideId && x.UserId == userId);
		}

		public HashSet<Guid> GetIdOfVisitedSlides(string courseId, string userId)
		{
			return new HashSet<Guid>(db.Visits.Where(x => x.UserId == userId && x.CourseId == courseId).Select(x => x.SlideId));
		}
		
		public bool HasVisitedSlides(string courseId, string userId)
		{
			return db.Visits.Any(x => x.UserId == userId && x.CourseId == courseId);
		}

		private async Task UpdateAttempts(Guid slideId, string userId, Action<Visit> action)
		{
			var visit = db.Visits.FirstOrDefault(v => v.SlideId == slideId && v.UserId == userId);
			if (visit == null)
				return;
			action(visit);
			await db.SaveChangesAsync();
		}

		public async Task RemoveAttempts(Guid slideId, string userId)
		{
			await UpdateAttempts(slideId, userId, visit =>
			{
				visit.AttemptsCount = 0;
				visit.Score = 0;
				visit.IsPassed = false;
			});
		}

		public async Task SetScoreForAttempt(Guid slideId, string userId, int newScore)
		{
			await UpdateAttempts(slideId, userId, visit =>
			{
				visit.Score = newScore;
				visit.IsPassed = true;
			});
		}

		public async Task AddAttempt(Guid slideId, string userId, int score)
		{
			await UpdateAttempts(slideId, userId, visit =>
			{
				visit.AttemptsCount++;
				visit.Score = score;
				visit.IsPassed = true;
			});
		}

		public async Task DropAttempt(Guid slideId, string userId)
		{
			await UpdateAttempts(slideId, userId, visit =>
			{
				visit.Score = 0;
				visit.IsPassed = false;
			});
		}

		public async Task AddSolutionAttempt(Guid slideId, string userId, bool isRightAnswer)
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

		public Dictionary<Guid, int> GetScoresForSlides(string courseId, string userId)
		{
			return db.Visits
				.Where(v => v.CourseId == courseId && v.UserId == userId)
				.GroupBy(v => v.SlideId, (s, visits) => new { Key = s, Value = visits.FirstOrDefault()})
				.ToDictionary(g => g.Key, g => g.Value.Score);
		}

		public int GetScore(Guid slideId, string userId)
		{
			return db.Visits
				.Where(v => v.SlideId == slideId && v.UserId == userId)
				.Select(v => v.Score)
				.FirstOrDefault();
		}

		public Visit GetVisiter(Guid slideId, string userId)
		{
			return db.Visits.FirstOrDefault(v => v.SlideId == slideId && v.UserId == userId);
		}

		public async Task SkipSlide(string courseId, Guid slideId, string userId)
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

		public bool IsSkipped(Guid slideId, string userId)
		{
			return db.Visits.Any(v => v.SlideId == slideId && v.UserId == userId && v.IsSkipped);
		}

		public bool IsPassed(Guid slideId, string userId)
		{
			return db.Visits.Any(v => v.SlideId == slideId && v.UserId == userId && v.IsPassed);
		}

		public bool IsSkippedOrPassed(Guid slideId, string userId)
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

		public IQueryable<Visit> GetVisitsInPeriod(IEnumerable<Guid> slidesIds, DateTime periodStart, DateTime periodFinish)
		{
			return db.Visits.Where(v => slidesIds.Contains(v.SlideId) && periodStart <= v.Timestamp && v.Timestamp <= periodFinish);
		}

		public Dictionary<Guid, List<Visit>> GetVisitsInPeriodForEachSlide(IEnumerable<Guid> slidesIds, DateTime periodStart, DateTime periodFinish)
		{
			return GetVisitsInPeriod(slidesIds, periodStart, periodFinish)
				.GroupBy(v => v.SlideId)
				.ToDictionary(g => g.Key, g => g.ToList());
		}

		public IEnumerable<string> GetUsersVisitedAllSlides(IImmutableSet<Guid> slidesIds, DateTime periodStart, DateTime periodFinish)
		{
			var slidesCount = slidesIds.Count;

			return GetVisitsInPeriod(slidesIds, periodStart, periodFinish)
				.DistinctBy(v => Tuple.Create(v.UserId, v.SlideId))
				.GroupBy(v => v.UserId)
				.Where(g => g.Count() == slidesCount)
				.Select(g => g.Key);
		}
	}
}