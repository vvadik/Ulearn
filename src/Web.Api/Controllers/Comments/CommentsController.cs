using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Database.Repos.Comments;
using Database.Repos.CourseRoles;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Ulearn.Common;
using Ulearn.Web.Api.Models.Parameters.Comments;
using Ulearn.Web.Api.Models.Responses;
using Ulearn.Web.Api.Models.Responses.Comments;

namespace Ulearn.Web.Api.Controllers.Comments
{
	[Route("comments/in/{courseId}/")]
	public class CommentsController : BaseController
	{
		private readonly ICommentsRepo commentsRepo;
		private readonly ICommentLikesRepo commentLikesRepo;
		private readonly ICourseRolesRepo courseRolesRepo;
		private readonly ICoursesRepo coursesRepo;

		public CommentsController(ILogger logger, IWebCourseManager courseManager, UlearnDb db,
			ICommentsRepo commentsRepo, ICommentLikesRepo commentLikesRepo,
			IUsersRepo usersRepo, ICourseRolesRepo courseRolesRepo, ICoursesRepo coursesRepo)
			: base(logger, courseManager, db, usersRepo)
		{
			this.commentsRepo = commentsRepo;
			this.commentLikesRepo = commentLikesRepo;
			this.courseRolesRepo = courseRolesRepo;
			this.coursesRepo = coursesRepo;
		}

		[HttpGet("{slideId:guid}")]
		public async Task<ActionResult<CommentsListResponse>> SlideComments(string courseId, Guid slideId, [FromQuery] SlideCommentsParameters parameters)
		{
			var comments = await commentsRepo.GetSlideTopLevelCommentsAsync(courseId, slideId).ConfigureAwait(false);
			comments = comments.Where(c => !c.IsForInstructorsOnly).ToList();
			var canUserSeeNotApprovedComments = await CanUserSeeNotApprovedCommentsAsync(UserId, courseId).ConfigureAwait(false);
			comments = FilterVisibleComments(comments, canUserSeeNotApprovedComments);
				
			var totalCount = comments.Count;
			comments = comments.Skip(parameters.Offset).Take(parameters.Count).ToList();

			var replies = await commentsRepo.GetRepliesAsync(comments.Select(c => c.Id)).ConfigureAwait(false);
			var allCommentsIds = comments.Concat(replies.SelectMany(g => g.Value)).Select(c => c.Id);
			var commentLikesCount = await commentLikesRepo.GetLikesCountsAsync(allCommentsIds).ConfigureAwait(false);
			
			return new CommentsListResponse
			{
				TopLevelComments = BuildCommentsListResponse(comments, true, canUserSeeNotApprovedComments, replies, commentLikesCount),
				PaginationResponse = new PaginationResponse
				{
					Offset = parameters.Offset,
					Count = comments.Count,
					TotalCount = totalCount,
				}
			};
		}

		private List<CommentInfo> BuildCommentsListResponse(
			IEnumerable<Comment> comments,
			bool isTopLevel, bool canUserSeeNotApprovedComments, DefaultDictionary<int, List<Comment>> replies, DefaultDictionary<int, int> commentLikesCount
		)
		{
			return comments.Select(c => BuildCommentInfo(c, isTopLevel, canUserSeeNotApprovedComments, replies, commentLikesCount)).ToList();
		}
		
		private CommentInfo BuildCommentInfo(
			Comment comment,
			bool isTopLevel, bool canUserSeeNotApprovedComments, DefaultDictionary<int, List<Comment>> replies, DefaultDictionary<int, int> commentLikesCount
		)
		{
			var commentInfo = new CommentInfo
			{
				Id = comment.Id,
				Text = comment.Text,
				Author = BuildShortUserInfo(comment.Author),
				PublishTime = comment.PublishTime,
				IsApproved = comment.IsApproved,
				LikesCount = commentLikesCount[comment.Id],
			};

			if (!isTopLevel)
			{
				commentInfo.IsCorrectAnswer = comment.IsCorrectAnswer;
				return commentInfo;
			}
			
			commentInfo.IsPinnedToTop = comment.IsPinnedToTop;
			var commentReplies = FilterVisibleComments(replies[comment.Id], canUserSeeNotApprovedComments);
			commentInfo.Replies = BuildCommentsListResponse(commentReplies, false, canUserSeeNotApprovedComments, null, commentLikesCount);
			
			return commentInfo;
		}

		private List<Comment> FilterVisibleComments(List<Comment> comments, bool canUserSeeNotApprovedComments)
		{
			return canUserSeeNotApprovedComments ? comments : comments.Where(c => c.IsApproved || c.AuthorId == UserId).ToList();
		}

		private async Task<bool> CanUserSeeNotApprovedCommentsAsync(string userId, string courseId)
		{
			var hasCourseAccessForCommentEditing = await coursesRepo.HasCourseAccessAsync(userId, courseId, CourseAccessType.EditPinAndRemoveComments).ConfigureAwait(false);
			var isCourseAdmin = await courseRolesRepo.HasUserAccessToCourseAsync(userId, courseId, CourseRoleType.CourseAdmin).ConfigureAwait(false);
			return isCourseAdmin || hasCourseAccessForCommentEditing;
		}
	}
}