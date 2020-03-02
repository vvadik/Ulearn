using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Repos;
using Database.Repos.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Ulearn.Core.Courses;
using Ulearn.Web.Api.Models.Responses.User;

namespace Ulearn.Web.Api.Controllers.User
{
	[Route("/user/progress")]
	public class UserProgressController : BaseController
	{
		private readonly IVisitsRepo visitsRepo;
		private readonly IUserQuizzesRepo userQuizzesRepo;
		private readonly IAdditionalScoresRepo additionalScoresRepo;

		public UserProgressController(ILogger logger, IWebCourseManager courseManager, UlearnDb db, IUsersRepo usersRepo,
			IVisitsRepo visitsRepo, IUserQuizzesRepo userQuizzesRepo, IAdditionalScoresRepo additionalScoresRepo)
			: base(logger, courseManager, db, usersRepo)
		{
			this.visitsRepo = visitsRepo;
			this.userQuizzesRepo = userQuizzesRepo;
			this.additionalScoresRepo = additionalScoresRepo;
		}

		/// <summary>
		/// Прогресс пользователя в курсе 
		/// </summary>
		[HttpGet("{courseId}")]
		[Authorize]
		public async Task<ActionResult<UserProgressResponse>> UserProgress(Course course)
		{
			var scores = visitsRepo.GetScoresForSlides(course.Id, UserId);
			var attempts = await userQuizzesRepo.GetUsedAttemptsCountAsync(course.Id, UserId).ConfigureAwait(false);
			var waitingSlides = await userQuizzesRepo.GetSlideIdsWaitingForManualCheckAsync(course.Id, UserId).ConfigureAwait(false);
			var additionalScores = await GetAdditionalScores(course.Id, UserId).ConfigureAwait(false);

			var slidesResults = scores.Select(s => new
			{
				Key = s.Key,
				Score = s.Value,
				UsedAttempts = attempts.GetValueOrDefault(s.Key),
				IsWaitingForManualChecking = waitingSlides.Contains(s.Key),
			}).ToDictionary(s => s.Key, s => new UserSlideResult
			{
				Visited = true,
				Score = s.Score,
				UsedAttempts = s.UsedAttempts,
				IsWaitingForManualChecking = s.IsWaitingForManualChecking,
			});

			return new UserProgressResponse
			{
				VisitedSlides = slidesResults,
				AdditionalScores = additionalScores
			};
		}
		
		private async Task<Dictionary<Guid, Dictionary<string, int>>> GetAdditionalScores(string courseId, string userId)
		{
			return (await additionalScoresRepo.GetAdditionalScoresForUser(courseId, userId).ConfigureAwait(false))
				.Select(kvp => (unitId: kvp.Key.Item1, scoringGroupId: kvp.Key.Item2, additionalScore: kvp.Value))
				.GroupBy(t => t.unitId)
				.ToDictionary(g => g.Key,
					g => g.ToDictSafe(t => t.scoringGroupId, t=> t.additionalScore));
		}

		/// <summary>
		/// Отметить посещение слайда в курсе
		/// </summary>
		/// <returns></returns>
		[HttpPost("{courseId}")]
		[Authorize]
		public async Task<ActionResult<UserProgressResponse>> Visit([FromRoute] Course course, [FromQuery] Guid slideId)
		{
			await visitsRepo.AddVisit(course.Id, slideId, UserId, GetRealClientIp());
			return await UserProgress(course);
		}

		private string GetRealClientIp()
		{
			var xForwardedFor = Request.Headers["X-Forwarded-For"].ToString();
			if (string.IsNullOrEmpty(xForwardedFor))
				return Request.Host.Host;
			return xForwardedFor.Split(',').FirstOrDefault() ?? "";
		}
	}
}