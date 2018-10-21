using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Database.Extensions;
using Database.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Ulearn.Common.Extensions;

namespace Database.Repos.Comments
{
	/* TODO (andgein): This repo is not fully migrated to .NET Core and EF Core */
	public class CommentsRepo : ICommentsRepo
	{
		private readonly UlearnDb db;
		private readonly ICommentPoliciesRepo commentPoliciesRepo;

		public CommentsRepo(UlearnDb db, ICommentPoliciesRepo commentPoliciesRepo)
		{
			this.db = db;
			this.commentPoliciesRepo = commentPoliciesRepo;
		}

		public async Task<Comment> AddCommentAsync(ClaimsPrincipal author, string courseId, Guid slideId, int parentCommentId, string commentText)
		{
			var commentsPolicy = await commentPoliciesRepo.GetCommentsPolicyAsync(courseId).ConfigureAwait(false);
			var isInstructor = author.HasAccessFor(courseId, CourseRole.Instructor);
			var isApproved = commentsPolicy.ModerationPolicy == CommentModerationPolicy.Postmoderation || isInstructor;

			/* Instructors' replies are automatically correct */
			var isReply = parentCommentId != -1;
			var isCorrectAnswer = isReply && isInstructor;
			
			var comment = new Comment
			{
				AuthorId = author.GetUserId(),
				CourseId = courseId,
				SlideId = slideId,
				ParentCommentId = parentCommentId,
				Text = commentText,
				IsApproved = isApproved,
				IsCorrectAnswer = isCorrectAnswer,
				PublishTime = DateTime.Now
			};
			db.Comments.Add(comment);
			await db.SaveChangesAsync();

			return await db.Comments.FindAsync(comment.Id);
		}

		[ItemCanBeNull]
		public Task<Comment> FindCommentByIdAsync(int commentId)
		{
			return db.Comments.FindAsync(commentId);
		}

		public Task<List<Comment>> GetCommentsByIdsAsync(IEnumerable<int> commentIds)
		{
			return db.Comments.Where(c => commentIds.Contains(c.Id)).ToListAsync();
		}

		/* Not asynced versions below */
		public Task<List<Comment>> GetSlideCommentsAsync(string courseId, Guid slideId)
		{
			return db.Comments.Where(x => x.SlideId == slideId && !x.IsDeleted).ToListAsync();
		}

		public Task<List<Comment>> GetSlidesCommentsAsync(string courseId, IEnumerable<Guid> slidesIds)
		{
			return db.Comments.Where(x => slidesIds.Contains(x.SlideId) && !x.IsDeleted).ToListAsync();
		}

		public Task<List<Comment>> GetCourseCommentsAsync(string courseId)
		{
			return db.Comments.Where(x => x.CourseId == courseId && !x.IsDeleted).ToListAsync();
		}

		public async Task<Comment> ModifyComment(int commentId, Action<Comment> modifyAction)
		{
			var comment = db.Comments.Find(commentId);
			modifyAction(comment);
			await db.SaveChangesAsync();
			return comment;
		}

		public async Task<Comment> EditCommentText(int commentId, string newText)
		{
			return await ModifyComment(commentId, c => c.Text = newText);
		}

		public async Task ApproveComment(int commentId, bool isApproved)
		{
			await ModifyComment(commentId, c => c.IsApproved = isApproved);
		}

		public async Task DeleteComment(int commentId)
		{
			await ModifyComment(commentId, c => c.IsDeleted = true);
		}

		public async Task<Comment> RestoreComment(int commentId)
		{
			return await ModifyComment(commentId, c => c.IsDeleted = false);
		}

		public async Task PinComment(int commentId, bool isPinned)
		{
			await ModifyComment(commentId, c => c.IsPinnedToTop = isPinned);
		}

		public async Task MarkCommentAsCorrectAnswer(int commentId, bool isCorrect = true)
		{
			await ModifyComment(commentId, c => c.IsCorrectAnswer = isCorrect);
		}

		public bool IsUserAddedMaxCommentsInLastTime(string userId, int maxCount, TimeSpan lastTime)
		{
			var lastComments = db.Comments.Where(x => x.AuthorId == userId)
				.OrderByDescending(x => x.PublishTime)
				.Take(maxCount)
				.OrderBy(x => x.PublishTime);
			if (lastComments.Count() < maxCount || maxCount <= 0)
				return false;
			var boundComment = lastComments.FirstOrDefault();
			return boundComment == null || boundComment.PublishTime >= DateTime.Now - lastTime;
		}
	}
}