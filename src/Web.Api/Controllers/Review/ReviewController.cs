using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Database.Repos.Groups;
using Database.Repos.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Ulearn.Common.Api.Models.Responses;
using Ulearn.Web.Api.Models.Parameters.Review;
using Ulearn.Web.Api.Models.Responses.Exercise;

namespace Ulearn.Web.Api.Controllers.Review
{
	public class ReviewController : BaseController
	{
		private readonly ISlideCheckingsRepo slideCheckingsRepo;
		private readonly ICourseRolesRepo courseRolesRepo;
		private readonly INotificationsRepo notificationsRepo;
		private readonly IUserSolutionsRepo userSolutionsRepo;
		private readonly IGroupAccessesRepo groupAccessesRepo;

		public ReviewController(IWebCourseManager courseManager, UlearnDb db, IUsersRepo usersRepo,
			ISlideCheckingsRepo slideCheckingsRepo,
			ICourseRolesRepo courseRolesRepo,
			IUserSolutionsRepo userSolutionsRepo,
			IGroupAccessesRepo groupAccessesRepo,
			INotificationsRepo notificationsRepo)
			: base(courseManager, db, usersRepo)
		{
			this.slideCheckingsRepo = slideCheckingsRepo;
			this.courseRolesRepo = courseRolesRepo;
			this.notificationsRepo = notificationsRepo;
			this.userSolutionsRepo = userSolutionsRepo;
			this.groupAccessesRepo = groupAccessesRepo;
		}

		/// <summary>
		/// Добавить ревью к решению
		/// </summary>
		[HttpPost("reviews")]
		[Authorize]
		[SwaggerResponse((int)HttpStatusCode.RequestEntityTooLarge, "Your comment is too large")]
		public async Task<ActionResult<ReviewInfo>> AddExerciseCodeReview([FromQuery] string submissionId, [FromBody] ReviewCreateParameters parameters)
		{
			var submission = await userSolutionsRepo.FindSubmissionById(submissionId);
			var checking = submission.ManualCheckings[0];//TODO not list

			if (!await groupAccessesRepo.CanInstructorViewStudentAsync(UserId, checking.UserId))
				return StatusCode((int)HttpStatusCode.Forbidden, "You don't have access to view this submission");

			if (parameters.StartLine > parameters.FinishLine || (parameters.StartLine == parameters.FinishLine && parameters.StartPosition > parameters.FinishPosition))
			{
				var tmp = parameters.StartLine;
				parameters.StartLine = parameters.FinishLine;
				parameters.FinishLine = tmp;

				tmp = parameters.StartPosition;
				parameters.StartPosition = parameters.FinishPosition;
				parameters.FinishPosition = tmp;
			}

			var review = await slideCheckingsRepo.AddExerciseCodeReview(int.Parse(submissionId), UserId, parameters.StartLine, parameters.StartPosition, parameters.FinishLine, parameters.FinishPosition, parameters.Text);
			return ReviewInfo.Build(review, null, false);
		}

		/// <summary>
		/// Удалить ревью
		/// </summary>
		[HttpDelete("reviews")]
		[Authorize]
		public async Task<ActionResult> DeleteExerciseCodeReviewComment([FromQuery] string submissionId, [FromQuery] int reviewId)
		{
			var submission = await userSolutionsRepo.FindSubmissionById(submissionId);
			var checking = submission.ManualCheckings[0];//TODO not list

			if (!await groupAccessesRepo.CanInstructorViewStudentAsync(UserId, checking.UserId))
				return StatusCode((int)HttpStatusCode.Forbidden, "You don't have access to view this submission");
			
			var review = checking.Reviews.FirstOrDefault(r=>r.Id == reviewId);
			if (review == null)
				return NotFound(new ErrorResponse($"Review {reviewId} not found"));

			var courseId = review.ExerciseCheckingId.HasValue ? review.ExerciseChecking.CourseId : review.Submission.CourseId;
			if (review.AuthorId != UserId && !await courseRolesRepo.HasUserAccessToCourse(UserId, courseId, CourseRoleType.CourseAdmin))
				return StatusCode((int)HttpStatusCode.Forbidden, new ErrorResponse("You can't delete this review"));

			await slideCheckingsRepo.DeleteExerciseCodeReview(review);

			return Ok(new SuccessResponseWithMessage($"Review {reviewId} successfully deleted"));
		}
	}
}