using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Database.Repos.CourseRoles;
using Database.Repos.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Ulearn.Common.Api.Models.Responses;
using Ulearn.Core;
using Ulearn.Core.Courses;
using Ulearn.Web.Api.Models.Common;
using Ulearn.Web.Api.Models.Responses.Courses;
using Ulearn.Web.Api.Models.Responses.User;

namespace Ulearn.Web.Api.Controllers.User
{
	[Route("/user/progress")]
	public class UserProgressController : BaseController
	{
		private readonly IVisitsRepo visitsRepo;
		private readonly IUserQuizzesRepo userQuizzesRepo;

		public UserProgressController(ILogger logger, IWebCourseManager courseManager, UlearnDb db, IUsersRepo usersRepo,
			IVisitsRepo visitsRepo, IUserQuizzesRepo userQuizzesRepo)
			: base(logger, courseManager, db, usersRepo)
		{
			this.visitsRepo = visitsRepo;
			this.userQuizzesRepo = userQuizzesRepo;
		}

		/// <summary>
		/// Прогресс пользователя в курсе 
		/// </summary>
		[HttpGet("{courseId}")]
		public async Task<ActionResult<UserProgressResponse>> UserProgress(Course course)
		{
			var scores = visitsRepo.GetScoresForSlides(course.Id, UserId);
			var attempts = userQuizzesRepo.GetUsedAttemptCountsAsync(course.Id, UserId);
			var waitingSlides = userQuizzesRepo.GetSlideIdsWaitingForManualCheckAsync(course.Id, UserId);

			await Task.WhenAll(attempts, waitingSlides).ConfigureAwait(false);

			var slidesResults = scores.Select(s => new
			{
				Key = s.Key,
				Score = s.Value,
				UsedAttempts = attempts.Result.ContainsKey(s.Key) ? attempts.Result[s.Key] : 0,
				IsWaitingForManualChecking = waitingSlides.Result.Contains(s.Key),
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
			};
		}
	}
}