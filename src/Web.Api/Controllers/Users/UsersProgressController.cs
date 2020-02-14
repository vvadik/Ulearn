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

namespace Ulearn.Web.Api.Controllers.Users
{
	[Route("/users/progress")]
	public class UsersProgressController : BaseController
	{
		private readonly IVisitsRepo visitsRepo;
		private readonly IUserQuizzesRepo userQuizzesRepo;

		public UsersProgressController(ILogger logger, IWebCourseManager courseManager, UlearnDb db, IUsersRepo usersRepo,
			IVisitsRepo visitsRepo, IUserQuizzesRepo userQuizzesRepo)
			: base(logger, courseManager, db, usersRepo)
		{
			this.visitsRepo = visitsRepo;
			this.userQuizzesRepo = userQuizzesRepo;
		}
		
		/// <summary>
		/// Прогресс пользователей в курсе 
		/// </summary>
		[HttpGet("{courseId}")]
		[Authorize(Policy = "CourseAdmins")]
		public async Task<ActionResult<UsersProgressResponse>> UsersProgress(Course course, [FromBody]List<string> userIds)
		{
			var shouldBeSolvedSlides = course.Slides.Where(s => s.ShouldBeSolved).Select(s => s.Id);
			var scores = await visitsRepo.GetScoresForSlides(course.Id, userIds, shouldBeSolvedSlides);

			var slidesWithScore = scores
				.Select(kvp =>
					new UserProgress
					{
						UserId = kvp.Key,
						SlidesWithScore = kvp.Value.ToDictionary(s => s.Key, s => new UserProgressSlideResult { Score = s.Value })
					}
				).ToList();

			return new UsersProgressResponse
			{
				SlidesWithScore = slidesWithScore
			};
		}
	}
}