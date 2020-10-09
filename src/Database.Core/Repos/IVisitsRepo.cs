using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Repos
{
	public interface IVisitsRepo
	{
		Task AddVisit(string courseId, Guid slideId, string userId, string ipAddress);
		Visit FindVisit(string courseId, Guid slideId, string userId);
		bool IsUserVisitedSlide(string courseId, Guid slideId, string userId);
		HashSet<Guid> GetIdOfVisitedSlides(string courseId, string userId);
		bool HasVisitedSlides(string courseId, string userId);
		Task UpdateScoreForVisit(string courseId, Guid slideId, int maxSlideScore, string userId);
		Task RemoveAttempts(string courseId, Guid slideId, string userId);
		Dictionary<Guid, int> GetScoresForSlides(string courseId, string userId, IEnumerable<Guid> slidesIds = null);
		Task<Dictionary<string, Dictionary<Guid, int>>> GetScoresForSlides(string courseId, IEnumerable<string> userIds, IEnumerable<Guid> slidesIds = null);
		Task<List<Guid>> GetSlidesWithUsersManualChecking(string courseId, string userId);
		Task MarkVisitsAsWithManualChecking(string courseId, Guid slideId, string userId);
		Task<int> GetScore(string courseId, Guid slideId, string userId);
		Task SkipSlide(string courseId, Guid slideId, string userId);
		Task<bool> IsSkipped(string courseId, Guid slideId, string userId);
		/// Забывает, что пользователь смотрел чужие решения и дает ему получить баллы при следующей отправке.
		Task UnskipAllSlides(string courseId, string userId);
		bool IsPassed(string courseId, Guid slideId, string userId);
		bool IsSkippedOrPassed(string courseId, Guid slideId, string userId);
		Task AddVisits(IEnumerable<Visit> visits);
		IQueryable<Visit> GetVisitsInPeriod(string courseId, IEnumerable<Guid> slidesIds, DateTime periodStart, DateTime periodFinish, IEnumerable<string> usersIds = null);
		IQueryable<Visit> GetVisitsInPeriod(VisitsFilterOptions options);
		Dictionary<Guid, List<Visit>> GetVisitsInPeriodForEachSlide(VisitsFilterOptions options);
		IEnumerable<string> GetUsersVisitedAllSlides(string courseId, IImmutableSet<Guid> slidesIds, DateTime periodStart, DateTime periodFinish, IEnumerable<string> usersIds = null);
		IEnumerable<string> GetUsersVisitedAllSlides(VisitsFilterOptions options);
		HashSet<string> GetUserCourses(string userId);
		Task<List<string>> GetCourseUsersAsync(string courseId);
		List<RatingEntry> GetCourseRating(string courseId, int minScore);
		Task<Dictionary<string, Visit>> FindLastVisit(List<string> userIds);
	}
}