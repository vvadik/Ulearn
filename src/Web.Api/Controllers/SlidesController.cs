using System;
using Database;
using Database.Repos;
using Database.Repos.Users;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Ulearn.Core.Courses;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Controllers
{
	[Route("/slides")]
	public class SlidesController : BaseController
	{
		public SlidesController(ILogger logger, IWebCourseManager courseManager, UlearnDb db, IUsersRepo usersRepo)
			: base(logger, courseManager, db, usersRepo)
		{
		}

		[HttpGet("{courseId}/{slideId}")]
		public ActionResult<ShortSlideInfo> SlideInfo(Course course, Guid slideId)
		{
			var slide = course?.FindSlideById(slideId);
			if (slide == null)
				return NotFound(new { status = "error", message = "Course or slide not found" });

			return BuildSlideInfo(course.Id, slide);
		}
	}
}