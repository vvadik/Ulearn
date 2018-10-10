using System;
using Database;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using uLearn;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Controllers
{
	[Route("/slides")]
	public class SlidesController : BaseController
	{
		public SlidesController(ILogger logger, WebCourseManager courseManager, UlearnDb db)
			: base(logger, courseManager, db)
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