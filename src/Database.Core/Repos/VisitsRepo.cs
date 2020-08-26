using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repos
{
	/* TODO (andgein): This repo is not fully migrated to .NET Core and EF Core */
	public class VisitsRepo : IVisitsRepo
	{
		private readonly UlearnDb db;
		private readonly ISlideCheckingsRepo slideCheckingsRepo;

		public VisitsRepo(UlearnDb db, ISlideCheckingsRepo slideCheckingsRepo)
		{
			this.db = db;
			this.slideCheckingsRepo = slideCheckingsRepo;
		}

		public async Task AddVisit(string courseId, Guid slideId, string userId, string ipAddress)
		{
			courseId = courseId.ToLower();
			await SetLastVisit(courseId, slideId, userId).ConfigureAwait(false);
			var visit = FindVisit(courseId, slideId, userId);
			if (visit == null)
			{
				db.Visits.Add(new Visit
				{
					UserId = userId,
					CourseId = courseId,
					SlideId = slideId,
					Timestamp = DateTime.Now,
					IpAddress = ipAddress,
				});
			}
			else if (visit.IpAddress != ipAddress)
				visit.IpAddress = ipAddress;

			await db.SaveChangesAsync();
		}

		private async Task SetLastVisit(string courseId, Guid slideId, string userId)
		{
			var lastVisit = FindLastVisit(courseId, userId);
			if (lastVisit == null)
			{
				db.LastVisits.Add(new LastVisit
				{
					UserId = userId,
					CourseId = courseId,
					SlideId = slideId,
					Timestamp = DateTime.Now,
				});
			}
			else
			{
				lastVisit.SlideId = slideId;
				lastVisit.Timestamp = DateTime.Now;
			}
			await db.SaveChangesAsync().ConfigureAwait(false);;
		}

		public Visit FindVisit(string courseId, Guid slideId, string userId)
		{
			return db.Visits.FirstOrDefault(v => v.CourseId == courseId && v.SlideId == slideId && v.UserId == userId);
		}

		public LastVisit FindLastVisit(string courseId, string userId)
		{
			return db.LastVisits.FirstOrDefault(v => v.CourseId == courseId && v.UserId == userId);
		}
		
		public async Task<Dictionary<string, Visit>> FindLastVisit(List<string> userIds)
		{
			return await db.Visits
				.Where(v => userIds.Contains(v.UserId))
				.Select(v => v.UserId)
				.Distinct()
				.Select(u => db.Visits
					.Where(v => v.UserId == u)
					.OrderByDescending(v => v.Timestamp)
					.FirstOrDefault()
				)
				.ToDictionaryAsync(v => v.UserId, v => v);
		}

		public bool IsUserVisitedSlide(string courseId, Guid slideId, string userId)
		{
			return FindVisit(courseId, slideId, userId) != null;
		}

		public HashSet<Guid> GetIdOfVisitedSlides(string courseId, string userId)
		{
			return new HashSet<Guid>(db.Visits.Where(v => v.UserId == userId && v.CourseId == courseId).Select(x => x.SlideId));
		}

		public bool HasVisitedSlides(string courseId, string userId)
		{
			return db.Visits.Any(v => v.CourseId == courseId && v.UserId == userId);
		}

		public async Task UpdateScoreForVisit(string courseId, Guid slideId, string userId)
		{
			var newScore = slideCheckingsRepo.GetManualScoreForSlide(courseId, slideId, userId) +
							slideCheckingsRepo.GetAutomaticScoreForSlide(courseId, slideId, userId);
			var isPassed = slideCheckingsRepo.IsSlidePassed(courseId, slideId, userId);
			await UpdateAttempts(courseId, slideId, userId, visit =>
			{
				visit.Score = newScore;
				visit.IsPassed = isPassed;
			});
		}

		private async Task UpdateAttempts(string courseId, Guid slideId, string userId, Action<Visit> action)
		{
			var visit = FindVisit(courseId, slideId, userId);
			if (visit == null)
			{
				await AddVisit(courseId, slideId, userId, null);
				visit = FindVisit(courseId, slideId, userId);
			}
			action(visit);
			await db.SaveChangesAsync();
		}

		public async Task RemoveAttempts(string courseId, Guid slideId, string userId)
		{
			await UpdateAttempts(courseId, slideId, userId, visit =>
			{
				visit.AttemptsCount = 0;
				visit.Score = 0;
				visit.IsPassed = false;
			});
		}

		public Dictionary<Guid, int> GetScoresForSlides(string courseId, string userId, IEnumerable<Guid> slidesIds = null)
		{
			var visits = db.Visits.Where(v => v.CourseId == courseId && v.UserId == userId);
			if (slidesIds != null)
				visits = visits.Where(v => slidesIds.Contains(v.SlideId));
			return visits
				.Select(v => new {v.SlideId, v.Score})
				.AsEnumerable()
				.GroupBy(v => v.SlideId, (s, v) => new { Key = s, Value = v.First().Score })
				.ToDictionary(g => g.Key, g => g.Value);
		}

		public async Task<Dictionary<string, Dictionary<Guid, int>>> GetScoresForSlides(string courseId, IEnumerable<string> userIds, IEnumerable<Guid> slidesIds = null)
		{
			var visits = db.Visits.Where(v => v.CourseId == courseId && userIds.Contains(v.UserId));
			if (slidesIds != null)
				visits = visits.Where(v => slidesIds.Contains(v.SlideId));
			return (await visits
				.Select(v => new {v.UserId, v.SlideId, v.Score})
				.ToListAsync().ConfigureAwait(false))
				.GroupBy(v => new {v.UserId, v.SlideId}, (s, v) => new { s.UserId, s.SlideId, Value = v.First().Score })
				.GroupBy(g => g.UserId)
				.ToDictionary(g => g.Key, g => g.ToDictionary(k => k.SlideId, k=> k.Value));
		}

		public List<Guid> GetSlidesWithUsersManualChecking(string courseId, string userId)
		{
			return db.Visits.Where(v => v.CourseId == courseId && v.UserId == userId)
				.Where(v => v.HasManualChecking)
				.Select(v => v.SlideId)
				.ToList();
		}

		public async Task MarkVisitsAsWithManualChecking(string courseId, Guid slideId, string userId)
		{
			await UpdateAttempts(courseId, slideId, userId, visit => { visit.HasManualChecking = true; });
		}

		public int GetScore(string courseId, Guid slideId, string userId)
		{
			return db.Visits
				.Where(v => v.CourseId == courseId && v.SlideId == slideId && v.UserId == userId)
				.Select(v => v.Score)
				.FirstOrDefault();
		}

		public async Task SkipSlide(string courseId, Guid slideId, string userId)
		{
			var visiter = FindVisit(courseId, slideId, userId);
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

		public bool IsSkipped(string courseId, Guid slideId, string userId)
		{
			return db.Visits.Any(v => v.CourseId == courseId && v.SlideId == slideId && v.UserId == userId && v.IsSkipped);
		}

		public async Task UnskipAllSlides(string courseId, string userId)
		{
			var skippedVisits = await db.Visits.Where(v => v.CourseId == courseId && v.UserId == userId && v.IsSkipped).ToListAsync();
			foreach (var v in skippedVisits)
				v.IsSkipped = false;
			await db.SaveChangesAsync();
		}

		public bool IsPassed(string courseId, Guid slideId, string userId)
		{
			return db.Visits.Any(v => v.CourseId == courseId && v.SlideId == slideId && v.UserId == userId && v.IsPassed);
		}

		public bool IsSkippedOrPassed(string courseId, Guid slideId, string userId)
		{
			return db.Visits.Any(v => v.CourseId == courseId && v.SlideId == slideId && v.UserId == userId && (v.IsPassed || v.IsSkipped));
		}

		public async Task AddVisits(IEnumerable<Visit> visits)
		{
			foreach (var visit in visits)
			{
				if (db.Visits.Any(x => x.UserId == visit.UserId && x.SlideId == visit.SlideId && x.CourseId == visit.CourseId))
					continue;
				db.Visits.Add(visit);
			}

			await db.SaveChangesAsync();
		}

		public IQueryable<Visit> GetVisitsInPeriod(string courseId, IEnumerable<Guid> slidesIds, DateTime periodStart, DateTime periodFinish, IEnumerable<string> usersIds = null)
		{
			var filteredVisits = db.Visits.Where(v => v.CourseId == courseId && slidesIds.Contains(v.SlideId) && periodStart <= v.Timestamp && v.Timestamp <= periodFinish);
			if (usersIds != null)
				filteredVisits = filteredVisits.Where(v => usersIds.Contains(v.UserId));
			return filteredVisits;
		}

		public IQueryable<Visit> GetVisitsInPeriod(VisitsFilterOptions options)
		{
			var filteredVisits = db.Visits.Where(v => options.PeriodStart <= v.Timestamp && v.Timestamp <= options.PeriodFinish);
			if (options.CourseId != null)
				filteredVisits = filteredVisits.Where(v => options.CourseId == v.CourseId);
			if (options.SlidesIds != null)
				filteredVisits = filteredVisits.Where(v => options.SlidesIds.Contains(v.SlideId));
			if (options.UserIds != null)
			{
				if (options.IsUserIdsSupplement)
					filteredVisits = filteredVisits.Where(v => !options.UserIds.Contains(v.UserId));
				else
					filteredVisits = filteredVisits.Where(v => options.UserIds.Contains(v.UserId));
			}

			return filteredVisits;
		}

		public Dictionary<Guid, List<Visit>> GetVisitsInPeriodForEachSlide(VisitsFilterOptions options)
		{
			return GetVisitsInPeriod(options)
				.AsEnumerable()
				.GroupBy(v => v.SlideId)
				.ToDictionary(g => g.Key, g => g.ToList());
		}

		public IEnumerable<string> GetUsersVisitedAllSlides(string courseId, IImmutableSet<Guid> slidesIds, DateTime periodStart, DateTime periodFinish, IEnumerable<string> usersIds = null)
		{
			var slidesCount = slidesIds.Count;

			return GetVisitsInPeriod(courseId, slidesIds, periodStart, periodFinish, usersIds)
				.Select(v => new { v.UserId, v.SlideId })
				.Distinct()
				.GroupBy(v => v.UserId)
				.Where(g => g.Count() == slidesCount)
				.Select(g => g.Key);
		}

		public IEnumerable<string> GetUsersVisitedAllSlides(VisitsFilterOptions options)
		{
			if (options.SlidesIds == null)
				throw new ArgumentNullException(nameof(options.SlidesIds));

			var slidesCount = options.SlidesIds.Count;

			return GetVisitsInPeriod(options)
				.Select(v => new { v.UserId, v.SlideId })
				.Distinct()
				.GroupBy(v => v.UserId)
				.Where(g => g.Count() == slidesCount)
				.Select(g => g.Key);
		}

		public HashSet<string> GetUserCourses(string userId)
		{
			return new HashSet<string>(db.Visits.Where(v => v.UserId == userId).Select(v => v.CourseId).Distinct());
		}

		public Task<List<string>> GetCourseUsersAsync(string courseId)
		{
			return db.Visits.Where(v => v.CourseId == courseId).Select(v => v.UserId).Distinct().ToListAsync();
		}

		public List<RatingEntry> GetCourseRating(string courseId, int minScore)
		{
			return db.Visits.Where(v => v.CourseId == courseId)
				.GroupBy(v => v.UserId)
				.Where(g => g.Sum(v => v.Score) >= minScore)
				.ToList()
				.Select(g => new RatingEntry(g.First().User, g.Sum(v => v.Score)))
				.OrderByDescending(r => r.Score)
				.ToList();
		}
	}

	public class RatingEntry
	{
		public RatingEntry(ApplicationUser user, int score)
		{
			User = user;
			Score = score;
		}

		public readonly ApplicationUser User;
		public readonly int Score;
	}
}