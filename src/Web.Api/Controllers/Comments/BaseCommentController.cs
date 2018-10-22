using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Database.Repos.Comments;
using Database.Repos.CourseRoles;
using Serilog;
using Ulearn.Common;
using Ulearn.Web.Api.Models.Responses.Comments;

namespace Ulearn.Web.Api.Controllers.Comments
{
	public class BaseCommentController : BaseController
	{
		protected readonly ICommentsRepo commentsRepo;
		protected readonly ICommentLikesRepo commentLikesRepo;
		protected readonly ICoursesRepo coursesRepo;
		protected readonly ICourseRolesRepo courseRolesRepo;

		public BaseCommentController(ILogger logger, IWebCourseManager courseManager, UlearnDb db, IUsersRepo usersRepo,
			ICommentsRepo commentsRepo, ICommentLikesRepo commentLikesRepo, ICoursesRepo coursesRepo, ICourseRolesRepo courseRolesRepo)
			: base(logger, courseManager, db, usersRepo)
		{
			this.commentsRepo = commentsRepo;
			this.commentLikesRepo = commentLikesRepo;
			this.coursesRepo = coursesRepo;
			this.courseRolesRepo = courseRolesRepo;
		}

		protected List<CommentInfo> BuildCommentsListResponse(IEnumerable<Comment> comments,
			bool canUserSeeNotApprovedComments, DefaultDictionary<int, List<Comment>> replies, DefaultDictionary<int, int> commentLikesCount,
			bool addCourseIdAndSlideId, bool addParentCommentId, bool addReplies)
		{
			return comments.Select(c => BuildCommentInfo(c, canUserSeeNotApprovedComments, replies, commentLikesCount, addCourseIdAndSlideId, addParentCommentId, addReplies)).ToList();
		}

		protected CommentInfo BuildCommentInfo(
			Comment comment,
			bool canUserSeeNotApprovedComments, DefaultDictionary<int, List<Comment>> replies, DefaultDictionary<int, int> commentLikesCount,
			bool addCourseIdAndSlideId, bool addParentCommentId, bool addReplies
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
			
			if (addCourseIdAndSlideId)
			{
				commentInfo.CourseId = comment.CourseId;
				commentInfo.SlideId = comment.SlideId;
			}

			if (addParentCommentId && !comment.IsTopLevel)
				commentInfo.ParentCommentId = comment.ParentCommentId;

			if (! comment.IsTopLevel)
			{
				commentInfo.IsCorrectAnswer = comment.IsCorrectAnswer;
				return commentInfo;
			}
			
			commentInfo.IsPinnedToTop = comment.IsPinnedToTop;
			if (addReplies)
			{
				var commentReplies = FilterVisibleComments(replies[comment.Id], canUserSeeNotApprovedComments);
				commentInfo.Replies = BuildCommentsListResponse(commentReplies, canUserSeeNotApprovedComments, null, commentLikesCount, addCourseIdAndSlideId, addParentCommentId, addReplies);
			}

			return commentInfo;
		}

		protected async Task<bool> CanUserSeeNotApprovedCommentsAsync(string userId, string courseId)
		{
			if (string.IsNullOrEmpty(userId))
				return false;
			
			var hasCourseAccessForCommentEditing = await coursesRepo.HasCourseAccessAsync(userId, courseId, CourseAccessType.EditPinAndRemoveComments).ConfigureAwait(false);
			var isCourseAdmin = await courseRolesRepo.HasUserAccessToCourseAsync(userId, courseId, CourseRoleType.CourseAdmin).ConfigureAwait(false);
			return isCourseAdmin || hasCourseAccessForCommentEditing;
		}

		protected List<Comment> FilterVisibleComments(List<Comment> comments, bool canUserSeeNotApprovedComments)
		{
			return canUserSeeNotApprovedComments ? comments : comments.Where(c => c.IsApproved || c.AuthorId == UserId).ToList();
		}
	}
}