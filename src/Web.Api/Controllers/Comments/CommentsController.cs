using System;
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
using Microsoft.AspNetCore.Mvc.Routing;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;
using Ulearn.Common.Extensions;
using Ulearn.Web.Api.Models.Parameters.Comments;
using Ulearn.Web.Api.Models.Responses;
using Ulearn.Web.Api.Models.Responses.Comments;

namespace Ulearn.Web.Api.Controllers.Comments
{
	[Route("comments/in/{courseId}/")]
	public class CommentsController : BaseCommentController
	{
		private readonly ICommentPoliciesRepo commentPoliciesRepo;
		private readonly INotificationsRepo notificationsRepo;

		public CommentsController(ILogger logger, IWebCourseManager courseManager, UlearnDb db,
			ICommentsRepo commentsRepo, ICommentLikesRepo commentLikesRepo, ICommentPoliciesRepo commentPoliciesRepo,
			IUsersRepo usersRepo, ICoursesRepo coursesRepo, ICourseRolesRepo courseRolesRepo, INotificationsRepo notificationsRepo)
			: base(logger, courseManager, db, usersRepo, commentsRepo, commentLikesRepo, coursesRepo, courseRolesRepo)
		{
			this.commentPoliciesRepo = commentPoliciesRepo;
			this.notificationsRepo = notificationsRepo;
		}

		/// <summary>
		/// Комментарии под слайдом
		/// </summary>
		[HttpGet("{slideId:guid}")]
		public async Task<ActionResult<CommentsListResponse>> SlideComments(string courseId, Guid slideId, [FromQuery] SlideCommentsParameters parameters)
		{
			var comments = await commentsRepo.GetSlideTopLevelCommentsAsync(courseId, slideId).ConfigureAwait(false);
			comments = comments.Where(c => !c.IsForInstructorsOnly).ToList();
			return await GetSlideCommentsResponseAsync(comments, courseId, parameters).ConfigureAwait(false);
		}

		/// <summary>
		/// Комментарии для преподавателей под слайдом
		/// </summary>
		[Authorize(Policy = "Instructors")]
		[HttpGet("{slideId:guid}/for_instructors")]
		public async Task<ActionResult<CommentsListResponse>> SlideCommentsForInstructors(string courseId, Guid slideId, [FromQuery] SlideCommentsParameters parameters)
		{
			var comments = await commentsRepo.GetSlideTopLevelCommentsAsync(courseId, slideId).ConfigureAwait(false);
			comments = comments.Where(c => c.IsForInstructorsOnly).ToList();
			return await GetSlideCommentsResponseAsync(comments, courseId, parameters).ConfigureAwait(false);
		}
		
		private async Task<ActionResult<CommentsListResponse>> GetSlideCommentsResponseAsync(List<Comment> comments, string courseId, SlideCommentsParameters parameters)
		{
			var canUserSeeNotApprovedComments = await CanUserSeeNotApprovedCommentsAsync(UserId, courseId).ConfigureAwait(false);
			comments = FilterVisibleComments(comments, canUserSeeNotApprovedComments);

			var totalCount = comments.Count;
			comments = comments.Skip(parameters.Offset).Take(parameters.Count).ToList();

			var replies = await commentsRepo.GetRepliesAsync(comments.Select(c => c.Id)).ConfigureAwait(false);
			var allCommentsIds = comments.Concat(replies.SelectMany(g => g.Value)).Select(c => c.Id);
			var commentLikesCount = await commentLikesRepo.GetLikesCountsAsync(allCommentsIds).ConfigureAwait(false);

			return new CommentsListResponse
			{
				TopLevelComments = BuildCommentsListResponse(comments, canUserSeeNotApprovedComments, replies, commentLikesCount, addCourseIdAndSlideId: false, addParentCommentId: false, addReplies: true),
				PaginationResponse = new PaginationResponse
				{
					Offset = parameters.Offset,
					Count = comments.Count,
					TotalCount = totalCount,
				}
			};
		}

		/// <summary>
		/// Добавить комментарий под слайдом
		/// </summary>
		[Authorize]
		[SwaggerResponse((int)HttpStatusCode.TooManyRequests, "You are commenting too fast. Please wait some time")]
		[SwaggerResponse((int)HttpStatusCode.RequestEntityTooLarge, "Your comment is too large")]
		[HttpPost("{slideId:guid}")]
		public async Task<ActionResult<CreateCommentResponse>> CreateComment(string courseId, Guid slideId, [FromBody] CreateCommentParameters parameters)
		{
			if (parameters.IsForInstructorsOnly)
			{
				var isInstructor = await courseRolesRepo.HasUserAccessToCourseAsync(UserId, courseId, CourseRoleType.Instructor).ConfigureAwait(false);
				if (!isInstructor)
					return StatusCode((int)HttpStatusCode.Forbidden, new ErrorResponse($"You can not create comment for instructors. You should be instructor or course admin of course {courseId}."));
			}

			if (parameters.ParentCommentId.HasValue)
			{
				var parentComment = await commentsRepo.FindCommentByIdAsync(parameters.ParentCommentId.Value).ConfigureAwait(false);
				if (parentComment == null || !parentComment.CourseId.EqualsIgnoreCase(courseId) || parentComment.SlideId != slideId || !parentComment.IsTopLevel)
					return BadRequest(new ErrorResponse($"`reply_to` comment {parameters.ParentCommentId.Value} not found, belongs to other course, other slide or is not a top-level comment"));
				

				if (parentComment.IsForInstructorsOnly != parameters.IsForInstructorsOnly)
					return BadRequest(new ErrorResponse(
						$"`reply_to` comment {parameters.ParentCommentId.Value} is {(parentComment.IsForInstructorsOnly ? "" : "not")} for instructors, but new one {(parameters.IsForInstructorsOnly ? "is" : "is not")}"
					));
			}
			
			var commentsPolicy = await commentPoliciesRepo.GetCommentsPolicyAsync(courseId).ConfigureAwait(false);
			
			if (!await CanCommentHereAsync(UserId, courseId, parameters.ParentCommentId.HasValue, commentsPolicy).ConfigureAwait(false))
				return StatusCode((int)HttpStatusCode.Forbidden, new ErrorResponse($"You can not create comment here by comments policy."));

			if (!await CanCommentNowAsync(UserId, courseId, commentsPolicy).ConfigureAwait(false))
				return StatusCode((int)HttpStatusCode.TooManyRequests, new ErrorResponse("You are commenting too fast. Please wait some time"));

			if (parameters.Text.Length > CommentsPolicy.MaxCommentLength)
				return StatusCode((int)HttpStatusCode.RequestEntityTooLarge, new ErrorResponse($"Your comment is too large. Max allowed length is {CommentsPolicy.MaxCommentLength} chars"));
			
			var parentCommentId = parameters.ParentCommentId ?? -1;
			var comment = await commentsRepo.AddCommentAsync(UserId, courseId, slideId, parentCommentId, parameters.IsForInstructorsOnly, parameters.Text).ConfigureAwait(false);
			
			if (comment.IsApproved)
				await NotifyAboutNewCommentAsync(comment).ConfigureAwait(false);

			var url = Url.Action(new UrlActionContext { Action = nameof(CommentController.Comment), Controller = "Comment", Values = new { commentId = comment.Id } });
			return new CreateCommentResponse
			{
				CommentId = comment.Id,
				ApiUrl = url,
			};
		}
		
		private async Task<bool> CanCommentNowAsync(string userId, string courseId, CommentsPolicy commentsPolicy)
		{
			/* Instructors have unlimited comments */
			if (await courseRolesRepo.HasUserAccessToCourseAsync(userId, courseId, CourseRoleType.Instructor).ConfigureAwait(false))
				return true;
			
			var isUserAddedMaxCommentsInLastTime = await commentsRepo.IsUserAddedMaxCommentsInLastTimeAsync(
				userId,
				commentsPolicy.MaxCommentsCountInLastTime,
				commentsPolicy.LastTimeForMaxCommentsLimit
			).ConfigureAwait(false);
			return !isUserAddedMaxCommentsInLastTime;
		}
		
		private async Task<bool> CanCommentHereAsync(string userId, string courseId, bool isReply, CommentsPolicy commentsPolicy)
		{
			var isInstructor = await courseRolesRepo.HasUserAccessToCourseAsync(userId, courseId, CourseRoleType.Instructor).ConfigureAwait(false);

			if (!isInstructor && !commentsPolicy.IsCommentsEnabled)
				return false;

			if (isReply && !isInstructor && commentsPolicy.OnlyInstructorsCanReply)
				return false;

			return true;
		}
		
		private async Task NotifyAboutNewCommentAsync(Comment comment)
		{
			var courseId = comment.CourseId;

			if (! comment.IsTopLevel)
			{
				var parentComment = await commentsRepo.FindCommentByIdAsync(comment.ParentCommentId).ConfigureAwait(false);
				if (parentComment != null)
				{
					var replyNotification = new RepliedToYourCommentNotification
					{
						Comment = comment,
						ParentComment = parentComment,
					};
					await notificationsRepo.AddNotificationAsync(courseId, replyNotification, comment.AuthorId).ConfigureAwait(false);
				}
			}

			/* Create NewComment[ForInstructors]Notification later than RepliedToYourCommentNotification, because the last one is blocker for the first one.
			 * We don't send NewComment[ForInstructors]Notification if RepliedToYouCommentNotification exists */
			var notification = comment.IsForInstructorsOnly
				? (Notification) new NewCommentForInstructorsOnlyNotification { Comment = comment } 
				: new NewCommentNotification { Comment = comment };
			await notificationsRepo.AddNotificationAsync(courseId, notification, comment.AuthorId).ConfigureAwait(false);
		}
	}
}