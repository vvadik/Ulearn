using System.Linq;
using System.Threading.Tasks;
using Database;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using uLearn;
using Ulearn.Web.Api.Models.Common;
using Ulearn.Web.Api.Models.Responses.Courses;

namespace Ulearn.Web.Api.Controllers
{
	[Route("/courses")]
	public class CoursesController : BaseController
	{
		public CoursesController(ILogger logger, WebCourseManager courseManager, UlearnDb db)
			: base(logger, courseManager, db)
		{
		}

		[HttpGet("")]
		public async Task<IActionResult> CoursesList()
		{
			var courses = await courseManager.GetCoursesAsync().ConfigureAwait(false);
			return Json(new CoursesListResponse
			{
				Courses = courses.Select(
					c => new ShortCourseInfo
					{
						Id = c.Id,
						Title = c.Title,
						ApiUrl = Url.Action(nameof(CourseInfo), "Courses", new { courseId = c.Id })
					}
				).ToList()
			});
		}
		
		[HttpGet("{courseId}")]
		public IActionResult CourseInfo(Course course)
		{
			if (course == null)
				return Json(new { status = "error", message = "Course not found" });
			
			return Json(new CourseInfo
			{
				Id = course.Id,
				Title = course.Title,
				Units = course.Units.Select(unit => BuildUnitInfo(course.Id, unit)).ToList()
			});
		}
	}
}