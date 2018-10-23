using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Database.Repos.Comments;
using Database.Repos.CourseRoles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NUnit.Framework;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;
using Ulearn.Common;
using Ulearn.Web.Api.Models.Parameters.Comments;
using Ulearn.Web.Api.Models.Responses;
using Ulearn.Web.Api.Models.Responses.Comments;

namespace Ulearn.Web.Api.Controllers.Comments
{
	[ProducesResponseType((int) HttpStatusCode.OK)]
	[SwaggerResponse((int) HttpStatusCode.Forbidden, "You don't have access to this comment")]
	[Route("comment/{commentId:int:min(0)}")]
	public class CommentController : BaseCommentController
	{
		private readonly INotificationsRepo notificationsRepo;

		public CommentController(ILogger logger, IWebCourseManager courseManager, UlearnDb db,
			IUsersRepo usersRepo, ICommentsRepo commentsRepo, ICommentLikesRepo commentLikesRepo, ICoursesRepo coursesRepo, ICourseRolesRepo courseRolesRepo,
			INotificationsRepo notificationsRepo)
			: base(logger, courseManager, db, usersRepo, commentsRepo, commentLikesRepo, coursesRepo, courseRolesRepo)
		{
			this.notificationsRepo = notificationsRepo;
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
		
		/// <summary>
		/// Информация о комментарии
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<CommentResponse>> Comment(int commentId, [FromQuery] CommentParameters parameters)
		{
			var comment = await commentsRepo.FindCommentByIdAsync(commentId).ConfigureAwait(false);
			var canUserSeeNotApprovedComments = await CanUserSeeNotApprovedCommentsAsync(UserId, comment.CourseId).ConfigureAwait(false);
			
			DefaultDictionary<int, int> likesCount;
			if (parameters.WithReplies)
			{
				var replies = await commentsRepo.GetRepliesAsync(commentId).ConfigureAwait(false);
				likesCount = await commentLikesRepo.GetLikesCountsAsync(replies.Append(comment).Select(c => c.Id)).ConfigureAwait(false);
				return BuildCommentResponse(
					comment,
					canUserSeeNotApprovedComments, new DefaultDictionary<int, List<Comment>> { {commentId, replies }}, likesCount,
					addCourseIdAndSlideId: true, addParentCommentId: true, addReplies: true
				);
			}
			
			likesCount = await commentLikesRepo.GetLikesCountsAsync(new [] { commentId }).ConfigureAwait(false);
			return BuildCommentResponse(
				comment,
				canUserSeeNotApprovedComments, new DefaultDictionary<int, List<Comment>>(), likesCount,
				addCourseIdAndSlideId: true, addParentCommentId: true, addReplies: false
			);
		}

		/// <summary>
		/// Лайки к комментарию
		/// </summary>
		[HttpGet("likes")]
		public async Task<ActionResult<CommentLikesResponse>> Likes(int commentId, [FromQuery] CommentLikesParameters parameters)
		{
			var likes = await commentLikesRepo.GetLikesAsync(commentId).ConfigureAwait(false);
			var paginatedLikes = likes.Skip(parameters.Offset).Take(parameters.Count).ToList();
			return new CommentLikesResponse
			{
				Likes = paginatedLikes.Select(like => new CommentLikeInfo
				{
					User = BuildShortUserInfo(like.User),
					Timestamp = like.Timestamp,
				}).ToList(),
				PaginationResponse = new PaginationResponse
				{
					Offset = parameters.Offset,
					Count = paginatedLikes.Count,
					TotalCount = likes.Count,
				}
			};
		}

		/// <summary>
		/// Лайкнуть комментарий
		/// </summary>
		[Authorize]
		[SwaggerResponse((int) HttpStatusCode.Conflict, "You have liked the comment already")]
		[HttpPost("like")]
		public async Task<IActionResult> Like(int commentId)
		{
			if (await commentLikesRepo.DidUserLikeComment(commentId, UserId).ConfigureAwait(false))
				return Conflict(new ErrorResponse($"You have liked the comment {commentId} already"));
					
			await commentLikesRepo.LikeAsync(commentId, UserId).ConfigureAwait(false);

			await NotifyAboutLikedComment(commentId).ConfigureAwait(false);
			
			return Ok(new SuccessResponse($"You have liked the comment {commentId}"));
		}

		/// <summary>
		/// Удалить лайк к комментарию
		/// </summary>
		[Authorize]
		[SwaggerResponse((int) HttpStatusCode.NotFound, "You don't have like for the comment")]
		[HttpDelete("like")]
		public async Task<IActionResult> Unlike(int commentId)
		{
			if (!await commentLikesRepo.DidUserLikeComment(commentId, UserId).ConfigureAwait(false))
				return NotFound(new ErrorResponse($"You don't have like for the comment {commentId}"));
			
			await commentLikesRepo.UnlikeAsync(commentId, UserId).ConfigureAwait(false);
			
			return Ok(new SuccessResponse($"You have unliked the comment {commentId}"));
		}
		
		private async Task NotifyAboutLikedComment(int commentId)
		{
			var comment = await commentsRepo.FindCommentByIdAsync(commentId).ConfigureAwait(false);
			if (comment != null)
			{
				var notification = new LikedYourCommentNotification
				{
					Comment = comment,
					LikedUserId = UserId,
				};
				await notificationsRepo.AddNotificationAsync(comment.CourseId, notification, UserId).ConfigureAwait(false);
			}
		}
	}
}