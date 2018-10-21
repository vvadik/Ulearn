using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Repos.Comments
{
	public interface ICommentsRepo
	{
		Task<Comment> AddCommentAsync(string authorId, string courseId, Guid slideId, int parentCommentId, string commentText);
		Task<Comment> FindCommentByIdAsync(int commentId);
		Task<List<Comment>> GetCommentsByIdsAsync(IEnumerable<int> commentIds);
		Task<List<Comment>> GetSlideCommentsAsync(string courseId, Guid slideId);
		Task<List<Comment>> GetSlidesCommentsAsync(string courseId, IEnumerable<Guid> slidesIds);
		Task<List<Comment>> GetCourseCommentsAsync(string courseId);
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