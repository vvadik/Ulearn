using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Database.Repos.CourseRoles;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Database.Repos.Comments
{
	/* This class is fully migrated to .NET Core and EF Core */
	public class CommentsRepo : ICommentsRepo
	{
		private readonly UlearnDb db;
		private readonly ICommentPoliciesRepo commentPoliciesRepo;
		private readonly ICourseRolesRepo courseRolesRepo;

		public CommentsRepo(UlearnDb db, ICommentPoliciesRepo commentPoliciesRepo, ICourseRolesRepo courseRolesRepo)
		{
			this.db = db;
			this.commentPoliciesRepo = commentPoliciesRepo;
			this.courseRolesRepo = courseRolesRepo;
		}
		
		public async Task<Comment> AddCommentAsync(string authorId, string courseId, Guid slideId, int parentCommentId, string commentText)
		{
			var commentsPolicy = await commentPoliciesRepo.GetCommentsPolicyAsync(courseId).ConfigureAwait(false);
			var isInstructor = await courseRolesRepo.HasUserAccessToCourseAsync(authorId, courseId, CourseRoleType.Instructor).ConfigureAwait(false);
			var isApproved = commentsPolicy.ModerationPolicy == CommentModerationPolicy.Postmoderation || isInstructor;

			/* Instructors' replies are automatically correct */
			var isReply = parentCommentId != -1;
			var isCorrectAnswer = isReply && isInstructor;
			
			var comment = new Comment
			{
				AuthorId = authorId,
				CourseId = courseId,
				SlideId = slideId,
				ParentCommentId = parentCommentId,
				Text = commentText,
				IsApproved = isApproved,
				IsCorrectAnswer = isCorrectAnswer,
				PublishTime = DateTime.Now
			};
			db.Comments.Add(comment);
			await db.SaveChangesAsync().ConfigureAwait(false);

			return await FindCommentByIdAsync(comment.Id).ConfigureAwait(false);
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

		public async Task<Comment> ModifyCommentAsync(int commentId, Action<Comment> modifyAction)
		{
			var comment = await FindCommentByIdAsync(commentId).ConfigureAwait(false);
			modifyAction(comment);
			await db.SaveChangesAsync().ConfigureAwait(false);
			return comment;
		}

		public Task<Comment> EditCommentTextAsync(int commentId, string newText)
		{
			return ModifyCommentAsync(commentId, c => c.Text = newText);
		}

		public Task<Comment> ApproveCommentAsync(int commentId, bool isApproved)
		{
			return ModifyCommentAsync(commentId, c => c.IsApproved = isApproved);
		}

		public Task DeleteCommentAsync(int commentId)
		{
			return ModifyCommentAsync(commentId, c => c.IsDeleted = true);
		}

		public Task<Comment> RestoreCommentAsync(int commentId)
		{
			return ModifyCommentAsync(commentId, c => c.IsDeleted = false);
		}

		public Task<Comment> PinCommentAsync(int commentId, bool isPinned)
		{
			return ModifyCommentAsync(commentId, c => c.IsPinnedToTop = isPinned);
		}

		public Task<Comment> MarkCommentAsCorrectAnswerAsync(int commentId, bool isCorrect = true)
		{
			return ModifyCommentAsync(commentId, c => c.IsCorrectAnswer = isCorrect);
		}

		public async Task<bool> IsUserAddedMaxCommentsInLastTimeAsync(string userId, int maxCount, TimeSpan lastTime)
		{
			var lastComments = db.Comments.Where(x => x.AuthorId == userId)
				.OrderByDescending(x => x.PublishTime)
				.Take(maxCount)
				.OrderBy(x => x.PublishTime);
			
			var commentsCount = await lastComments.CountAsync().ConfigureAwait(false);
			if (commentsCount < maxCount || maxCount <= 0)
				return false;
			
			var boundComment = await lastComments.FirstOrDefaultAsync().ConfigureAwait(false);
			return boundComment == null || boundComment.PublishTime >= DateTime.Now - lastTime;
		}
	}
}