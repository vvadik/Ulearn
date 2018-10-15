using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Repos;
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
		private readonly ICoursesRepo coursesRepo;

		public CoursesController(ILogger logger, WebCourseManager courseManager, UlearnDb db, ICoursesRepo coursesRepo)
			: base(logger, courseManager, db)
		{
			this.coursesRepo = coursesRepo;
		}

		/// <summary>
		/// Список курсов
		/// </summary>
		[HttpGet("")]
		public async Task<ActionResult<CoursesListResponse>> CoursesList()
		{
			var courses = await courseManager.GetCoursesAsync(coursesRepo).ConfigureAwait(false);
			return new CoursesListResponse
			{
				Courses = courses.Select(
					c => new ShortCourseInfo
					{
						Id = c.Id,
						Title = c.Title,
						ApiUrl = Url.Action(nameof(CourseInfo), "Courses", new { courseId = c.Id })
					}
				).ToList()
			};
		}
		
		/// <summary>
		/// Информация о курсе
		/// </summary>
		[HttpGet("{courseId}")]
		public ActionResult<CourseInfo> CourseInfo(Course course)
		{
			if (course == null)
				return Json(new { status = "error", message = "Course not found" });
			
			return new CourseInfo
			{
				Id = course.Id,
				Title = course.Title,
				Units = course.Units.Select(unit => BuildUnitInfo(course.Id, unit)).ToList()
			};
		}
	}
}