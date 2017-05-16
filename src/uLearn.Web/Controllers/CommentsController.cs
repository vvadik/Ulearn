using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using Database;
using Database.DataContexts;
using Database.Extensions;
using Database.Models;
using Microsoft.AspNet.Identity;
using uLearn.Extensions;
using uLearn.Web.FilterAttributes;
using uLearn.Web.Models;
using uLearn.Web.Telegram;

namespace uLearn.Web.Controllers
{
	public class CommentsController : Controller
	{
		private readonly CourseManager courseManager = WebCourseManager.Instance;
		private readonly CommentsRepo commentsRepo;
		private readonly NotificationsRepo notificationsRepo;
		private readonly VisitsRepo visitsRepo;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly CommentsBot commentsBot = new CommentsBot();

		public CommentsController()
		{
			var db = new ULearnDb();
			commentsRepo = new CommentsRepo(db);
			userManager = new ULearnUserManager(db);
			notificationsRepo = new NotificationsRepo(db);
			visitsRepo = new VisitsRepo(db);
		}

		public ActionResult SlideComments(string courseId, Guid slideId)
		{
			var slide = courseManager.GetCourse(courseId).GetSlideById(slideId);
			var comments = commentsRepo.GetSlideComments(courseId, slideId).ToList();
			var commentsPolicy = commentsRepo.GetCommentsPolicy(courseId);

			var commentsByParent = comments.GroupBy(x => x.ParentCommentId)
				.ToDictionary(x => x.Key, x => x.OrderBy(c => c.PublishTime).ToList());

			/* Top-level comments (with ParentCommentId = -1)
			   are sorting by publish time, but pinned comments are always higher */
			List<Comment> topLevelComments;
			if (commentsByParent.ContainsKey(-1))
				topLevelComments = commentsByParent.Get(-1)
					.OrderBy(x => !x.IsPinnedToTop).ThenBy(x => x.PublishTime).ToList();
			else
				topLevelComments = new List<Comment>();

			var commentsLikesCounts = commentsRepo.GetCommentsLikesCounts(comments);
			var commentsLikedByUser = commentsRepo.GetSlideCommentsLikedByUser(courseId, slideId, User.Identity.GetUserId()).ToImmutableHashSet();

			var isInstructor = User.HasAccessFor(courseId, CourseRole.Instructor);
			var isAuthorizedAndCanComment = CanAddCommentHere(User, courseId, false);
			var canReply = CanAddCommentHere(User, courseId, true);
			var canModerateComments = User.Identity.IsAuthenticated && isInstructor;
			var canSeeNotApprovedComments = User.Identity.IsAuthenticated && isInstructor;

			var model = new SlideCommentsModel
			{
				CourseId = courseId,
				Slide = slide,
				IsAuthorizedAndCanComment = isAuthorizedAndCanComment,
				CanReply = canReply,
				CanModerateComments = canModerateComments,
				CanSeeNotApprovedComments = canSeeNotApprovedComments,
				TopLevelComments = topLevelComments,
				CommentsByParent = commentsByParent,
				CommentsLikesCounts = commentsLikesCounts,
				CommentsLikedByUser = commentsLikedByUser,
				CurrentUser = User.Identity.IsAuthenticated ? userManager.FindById(User.Identity.GetUserId()) : null,
				CommentsPolicy = commentsPolicy,
			};
			return PartialView(model);
		}

		private bool CanAddCommentHere(IPrincipal user, string courseId, bool isReply)
		{
			if (!User.Identity.IsAuthenticated)
				return false;

			var commentsPolicy = commentsRepo.GetCommentsPolicy(courseId);
			var isInstructor = user.HasAccessFor(courseId, CourseRole.Instructor);

			if (!isInstructor && !commentsPolicy.IsCommentsEnabled)
				return false;

			if (isReply && !isInstructor && commentsPolicy.OnlyInstructorsCanReply)
				return false;

			return true;
		}

		private bool CanAddCommentNow(IPrincipal user, string courseId)
		{
			// Instructors have unlimited comments
			if (user.HasAccessFor(courseId, CourseRole.Instructor))
				return true;

			var commentsPolicy = commentsRepo.GetCommentsPolicy(courseId);
			return !commentsRepo.IsUserAddedMaxCommentsInLastTime(user.Identity.GetUserId(),
				commentsPolicy.MaxCommentsCountInLastTime,
				commentsPolicy.LastTimeForMaxCommentsLimit);
		}

		[ULearnAuthorize]
		[HttpPost]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> AddComment(string courseId, Guid slideId, string commentText, string parentCommentId)
		{
			var parentCommentIdInt = -1;
			if (parentCommentId != null)
				int.TryParse(parentCommentId, out parentCommentIdInt);

			if (!CanAddCommentHere(User, courseId, parentCommentIdInt != -1))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			if (!CanAddCommentNow(User, courseId))
			{
				return Json(new
				{
					status = "too-fast",
					message = "Вы комментируете слишком быстро. Подождите немного...",
				});
			}

			if (commentText.Length > CommentsPolicy.MaxCommentLength)
			{
				return Json(new
				{
					status = "too-long",
					message = "Слишком длинный комментарий. Попробуйте сократить мысль.",
				});
			}

			var comment = await commentsRepo.AddComment(User, courseId, slideId, parentCommentIdInt, commentText);
			await commentsBot.PostToChannel(comment);
			await NotifyAboutNewComment(comment);
			var canReply = CanAddCommentHere(User, courseId, isReply: true);

			return PartialView("_Comment", new CommentViewModel
			{
				Comment = comment,
				LikesCount = 0,
				IsLikedByUser = false,
				Replies = new List<CommentViewModel>(),
				IsCommentVisibleForUser = true,
				CanEditAndDeleteComment = true,
				CanModerateComment = User.HasAccessFor(courseId, CourseRole.Instructor),
				CanReply = canReply,
				CurrentUser = userManager.FindById(User.Identity.GetUserId()),
			});
		}

		private async Task NotifyAboutNewComment(Comment comment)
		{
			var courseId = comment.CourseId;
			var notification = new NewCommentNotification
			{
				Comment = comment,
			};
			await notificationsRepo.SendNotification(courseId, notification, comment.AuthorId, visitsRepo.GetCourseUsers(comment.CourseId));

			if (!comment.IsTopLevel())
			{
				var parentComment = commentsRepo.FindCommentById(comment.ParentCommentId);
				if (parentComment != null)
				{
					var replyNotification = new RepliedToYourCommentNotification
					{
						Comment = comment,
						ParentComment = parentComment,
					};
					await notificationsRepo.SendNotification(courseId, replyNotification, comment.AuthorId, parentComment.AuthorId);
				}
			}
		}

		[ULearnAuthorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> LikeComment(int commentId)
		{
			var userId = User.Identity.GetUserId();
			var res = await commentsRepo.LikeComment(commentId, userId);

			await NotifyAboutLikedComment(commentId);

			return Json(new { likesCount = res.Item1, liked = res.Item2 });
		}

		private async Task NotifyAboutLikedComment(int commentId)
		{
			var comment = commentsRepo.FindCommentById(commentId);
			if (comment != null)
			{
				var userId = User.Identity.GetUserId();
				var notification = new LikedYourCommentNotification
				{
					Comment = comment,
					LikedUserId = userId,
				};
				await notificationsRepo.SendNotification(comment.CourseId, notification, userId, comment.AuthorId);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ApproveComment(int commentId, bool isApproved = true)
		{
			var comment = commentsRepo.FindCommentById(commentId);
			if (comment == null)
				return HttpNotFound();

			if (!User.HasAccessFor(comment.CourseId, CourseRole.Instructor))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			await commentsRepo.ApproveComment(commentId, isApproved);
			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> PinComment(int commentId, bool isPinned)
		{
			var comment = commentsRepo.FindCommentById(commentId);
			if (comment == null)
				return HttpNotFound();

			if (!User.HasAccessFor(comment.CourseId, CourseRole.Instructor))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			await commentsRepo.PinComment(commentId, isPinned);
			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		private bool CanEditAndDeleteComment(IPrincipal user, Comment comment)
		{
			if (comment == null)
				return false;

			return user.HasAccessFor(comment.CourseId, CourseRole.Instructor) ||
					user.Identity.GetUserId() == comment.AuthorId;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteComment(int commentId)
		{
			var comment = commentsRepo.FindCommentById(commentId);
			if (!CanEditAndDeleteComment(User, comment))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			await commentsRepo.DeleteComment(commentId);
			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> RestoreComment(int commentId)
		{
			var comment = commentsRepo.FindCommentById(commentId);
			if (!CanEditAndDeleteComment(User, comment))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			await commentsRepo.RestoreComment(commentId);
			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		[ValidateInput(false)]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> EditCommentText(int commentId, string newText)
		{
			var comment = commentsRepo.FindCommentById(commentId);
			if (!CanEditAndDeleteComment(User, comment))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			var newComment = await commentsRepo.EditCommentText(commentId, newText);
			return PartialView("_CommentText", newComment);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> MarkAsCorrectAnswer(int commentId, bool isCorrect = true)
		{
			var comment = commentsRepo.FindCommentById(commentId);
			if (!CanEditAndDeleteComment(User, comment))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			await commentsRepo.MarkCommentAsCorrectAnswer(commentId, isCorrect);
			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}
	}

	public class SlideCommentsModel
	{
		public string CourseId { get; set; }
		public Slide Slide { get; set; }
		public bool IsAuthorizedAndCanComment { get; set; }
		public bool CanReply { get; set; }
		public bool CanModerateComments { get; set; }
		public bool CanSeeNotApprovedComments { get; set; }
		public List<Comment> TopLevelComments { get; set; }
		public Dictionary<int, List<Comment>> CommentsByParent { get; set; }
		public Dictionary<int, int> CommentsLikesCounts { get; set; }
		public ImmutableHashSet<int> CommentsLikedByUser { get; set; }
		public ApplicationUser CurrentUser { get; set; }
		public CommentsPolicy CommentsPolicy { get; set; }
	}
}