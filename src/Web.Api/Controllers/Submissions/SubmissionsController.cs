using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Database.Repos.Users;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Ulearn.Common.Api.Models.Responses;
using Ulearn.Web.Api.Models.Responses.Submissions;

namespace Ulearn.Web.Api.Controllers.Submissions
{
	[Route("/submissions")]
	public class SubmissionsController : BaseController
	{
		private readonly IUserSolutionsRepo userSolutionsRepo;
		private readonly ISlideCheckingsRepo slideCheckingsRepo;
		private readonly ICourseRolesRepo courseRolesRepo;

		public SubmissionsController(
			IWebCourseManager courseManager, UlearnDb db,
			IUsersRepo usersRepo,
			IUserSolutionsRepo userSolutionsRepo,
			ICourseRolesRepo courseRolesRepo,
			ISlideCheckingsRepo slideCheckingsRepo)
			: base(courseManager, db, usersRepo)
		{
			this.courseRolesRepo = courseRolesRepo;
			this.userSolutionsRepo = userSolutionsRepo;
			this.slideCheckingsRepo = slideCheckingsRepo;
		}

		[HttpGet]
		[Authorize]
		[SwaggerResponse((int)HttpStatusCode.Forbidden, "You don't have access to view submissions")]
		public async Task<ActionResult<SubmissionsResponse>> GetSubmissions([FromQuery] [CanBeNull] string userId, [FromQuery] string courseId, [FromQuery] Guid slideId)
		{
			var isCourseAdmin = await courseRolesRepo.HasUserAccessToCourse(UserId, courseId, CourseRoleType.CourseAdmin);
			if (userId != null && !isCourseAdmin)
			{
				var isInstructor = await courseRolesRepo.HasUserAccessToCourse(UserId, courseId, CourseRoleType.Instructor);
				if (!isInstructor)
					return StatusCode((int)HttpStatusCode.Forbidden, new ErrorResponse("You don't have access to view submissions"));
			}
			else
				userId = UserId;

			var submissions = await userSolutionsRepo
				.GetAllSubmissionsByUser(courseId, slideId, userId)
				.ToListAsync();
			var submissionsScores = await slideCheckingsRepo.GetCheckedPercentsBySubmissions(courseId, slideId, userId, null);
			var codeReviewComments = await slideCheckingsRepo.GetExerciseCodeReviewComments(courseId, slideId, userId);
			var reviewId2Comments = codeReviewComments
				?.GroupBy(c => c.ReviewId)
				.ToDictionary(g => g.Key, g => g.AsEnumerable());
			
			return SubmissionsResponse.Build(submissions, submissionsScores, reviewId2Comments, isCourseAdmin);
		}
	}
}