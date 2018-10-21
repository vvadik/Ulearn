using System;
using System.Threading.Tasks;
using Database;
using Database.Repos;
using Database.Repos.Comments;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Ulearn.Web.Api.Controllers.Comments
{
	public class CommentsController : BaseController
	{
		private readonly ICommentsRepo commentsRepo;

		public CommentsController(ILogger logger, IWebCourseManager courseManager, UlearnDb db,
			ICommentsRepo commentsRepo, IUsersRepo usersRepo)
			: base(logger, courseManager, db, usersRepo)
		{
			this.commentsRepo = commentsRepo;
		}

		[HttpGet("comments/in/{courseId}/{slideId:guid}")]
		public async Task<ActionResult<CommentsListResponse>> SlideComments(string courseId, Guid slideId)
		{
			var comments = await commentsRepo.GetSlideCommentsAsync(courseId, slideId).ConfigureAwait(false);
			return null;
		}
	}

	public class CommentsListResponse
	{
	}
}