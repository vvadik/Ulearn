using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Database.Repos.Comments;
using Database.Repos.CourseRoles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Ulearn.Web.Api.Models.Parameters.Comments;
using Ulearn.Web.Api.Models.Responses;
using Ulearn.Web.Api.Models.Responses.Comments;

namespace Ulearn.Web.Api.Controllers.Comments
{
	[Route("comments/in/{courseId}/")]
	public class CommentsController : BaseCommentController
	{
		public CommentsController(ILogger logger, IWebCourseManager courseManager, UlearnDb db,
			ICommentsRepo commentsRepo, ICommentLikesRepo commentLikesRepo,
			IUsersRepo usersRepo, ICoursesRepo coursesRepo, ICourseRolesRepo courseRolesRepo)
			: base(logger, courseManager, db, usersRepo, commentsRepo, commentLikesRepo, coursesRepo, courseRolesRepo)
		{
		}

		[HttpGet("{slideId:guid}")]
		public async Task<ActionResult<CommentsListResponse>> SlideComments(string courseId, Guid slideId, [FromQuery] SlideCommentsParameters parameters)
		{
			var comments = await commentsRepo.GetSlideTopLevelCommentsAsync(courseId, slideId).ConfigureAwait(false);
			comments = comments.Where(c => !c.IsForInstructorsOnly).ToList();
			return await GetSlideCommentsResponseAsync(courseId, parameters, comments).ConfigureAwait(false);
		}
		
		[Authorize(Policy = "Instructors")]
		[HttpGet("{slideId:guid}/for_instructors")]
		public async Task<ActionResult<CommentsListResponse>> SlideCommentsForInstructors(string courseId, Guid slideId, [FromQuery] SlideCommentsParameters parameters)
		{
			var comments = await commentsRepo.GetSlideTopLevelCommentsAsync(courseId, slideId).ConfigureAwait(false);
			comments = comments.Where(c => c.IsForInstructorsOnly).ToList();
			return await GetSlideCommentsResponseAsync(courseId, parameters, comments).ConfigureAwait(false);
		}
		
		private async Task<ActionResult<CommentsListResponse>> GetSlideCommentsResponseAsync(string courseId, SlideCommentsParameters parameters, List<Comment> comments)
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
	}
}