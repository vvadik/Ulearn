using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Repos
{
	public interface ICommentsRepo
	{
		Task<Comment> AddCommentAsync(ClaimsPrincipal author, string courseId, Guid slideId, int parentCommentId, string commentText);
		Task<Comment> FindCommentByIdAsync(int commentId);
		Task<List<Comment>> GetCommentsByIdsAsync(IEnumerable<int> commentIds);
		IEnumerable<Comment> GetSlideComments(string courseId, Guid slideId);
		IEnumerable<Comment> GetSlidesComments(string courseId, IEnumerable<Guid> slidesIds);
		IEnumerable<Comment> GetCourseComments(string courseId);

		///<returns>(likesCount, isLikedByThisUsed)</returns>
		Task<Tuple<int, bool>> LikeComment(int commentId, string userId);

		IEnumerable<CommentLike> GetCommentLikes(Comment comment);
		IEnumerable<ApplicationUser> GetCommentLikers(Comment comment);
		int GetCommentLikesCount(Comment comment);

		/// <returns>{commentId => likesCount}</returns>
		Dictionary<int, int> GetCommentsLikesCounts(IEnumerable<Comment> comments);

		IEnumerable<int> GetSlideCommentsLikedByUser(string courseId, Guid slideId, string userId);
		IEnumerable<int> GetCourseCommentsLikedByUser(string courseId, string userId);
		CommentsPolicy GetCommentsPolicy(string courseId);
		Task SaveCommentsPolicy(CommentsPolicy policy);
		Task<Comment> ModifyComment(int commentId, Action<Comment> modifyAction);
		Task<Comment> EditCommentText(int commentId, string newText);
		Task ApproveComment(int commentId, bool isApproved);
		Task DeleteComment(int commentId);
		Task<Comment> RestoreComment(int commentId);
		Task PinComment(int commentId, bool isPinned);
		Task MarkCommentAsCorrectAnswer(int commentId, bool isCorrect = true);
		bool IsUserAddedMaxCommentsInLastTime(string userId, int maxCount, TimeSpan lastTime);
	}
}