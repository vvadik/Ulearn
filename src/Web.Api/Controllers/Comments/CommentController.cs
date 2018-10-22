using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Database.Repos.Comments;
using Database.Repos.CourseRoles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;
using Ulearn.Common;
using Ulearn.Web.Api.Models.Parameters.Comments;
using Ulearn.Web.Api.Models.Responses;
using Ulearn.Web.Api.Models.Responses.Comments;

namespace Ulearn.Web.Api.Controllers.Comments
{
	[SwaggerResponse((int) HttpStatusCode.Forbidden, "You don't have access to this comment")]
	[Route("comment/{commentId:int:min(0)}")]
	public class CommentController : BaseCommentController
	{
		public CommentController(ILogger logger, IWebCourseManager courseManager, UlearnDb db, IUsersRepo usersRepo, ICommentsRepo commentsRepo, ICommentLikesRepo commentLikesRepo, ICoursesRepo coursesRepo, ICourseRolesRepo courseRolesRepo)
			: base(logger, courseManager, db, usersRepo, commentsRepo, commentLikesRepo, coursesRepo, courseRolesRepo)
		{
		}

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var commentId = (int) context.ActionArguments["commentId"];
			var comment = await commentsRepo.FindCommentByIdAsync(commentId).ConfigureAwait(false);

			if (comment == null)
			{
				context.Result = NotFound(new ErrorResponse("Comment not found"));
				return;
			}
			
			if (!comment.IsApproved)
			{
				var isAuthenticated = User.Identity.IsAuthenticated;
				var canUserSeeNotApprovedComments = await CanUserSeeNotApprovedCommentsAsync(UserId, comment.CourseId).ConfigureAwait(false);
				if (!isAuthenticated || !canUserSeeNotApprovedComments)
				{
					context.Result = StatusCode((int)HttpStatusCode.Forbidden, new ErrorResponse("You have no access to this comment"));
					return;
				}
			}

			if (comment.IsForInstructorsOnly)
			{
				var isInstructor = await courseRolesRepo.HasUserAccessToCourseAsync(UserId, comment.CourseId, CourseRoleType.Instructor).ConfigureAwait(false);
				if (!isInstructor)
				{
					context.Result = StatusCode((int)HttpStatusCode.Forbidden, new ErrorResponse("You have no access to this comment"));
					return;
				}
			}

			context.ActionArguments["comment"] = comment;
			
			 await base.OnActionExecutionAsync(context, next).ConfigureAwait(false);
		}
		
		[HttpGet]
		public async Task<ActionResult<CommentInfo>> Comment(int commentId, [FromQuery] CommentParameters parameters)
		{
			var comment = await commentsRepo.FindCommentByIdAsync(commentId).ConfigureAwait(false);
			var canUserSeeNotApprovedComments = await CanUserSeeNotApprovedCommentsAsync(UserId, comment.CourseId).ConfigureAwait(false);
			
			DefaultDictionary<int, int> likesCount;
			if (parameters.WithReplies)
			{
				var replies = await commentsRepo.GetRepliesAsync(commentId).ConfigureAwait(false);
				likesCount = await commentLikesRepo.GetLikesCountsAsync(replies.Append(comment).Select(c => c.Id)).ConfigureAwait(false);
				return BuildCommentInfo(
					comment,
					canUserSeeNotApprovedComments, new DefaultDictionary<int, List<Comment>> { {commentId, replies }}, likesCount,
					addCourseIdAndSlideId: true, addParentCommentId: true, addReplies: true
				);
			}
			
			likesCount = await commentLikesRepo.GetLikesCountsAsync(new [] { commentId }).ConfigureAwait(false);
			return BuildCommentInfo(
				comment,
				canUserSeeNotApprovedComments, new DefaultDictionary<int, List<Comment>>(), likesCount,
				addCourseIdAndSlideId: true, addParentCommentId: true, addReplies: false
			);
		}
	}
}