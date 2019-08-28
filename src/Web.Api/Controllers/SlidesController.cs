using System;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos.CourseRoles;
using Database.Repos.Users;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Controllers
{
	[Route("/slides")]
	public class SlidesController : BaseController
	{
		protected readonly ICourseRolesRepo CourseRolesRepo;
		
		public SlidesController(ILogger logger, IWebCourseManager courseManager, UlearnDb db, IUsersRepo usersRepo, ICourseRolesRepo courseRolesRepo)
			: base(logger, courseManager, db, usersRepo)
		{
			CourseRolesRepo = courseRolesRepo;
		}

		/// <summary>
		/// Информация о слайде, достаточная для отображения списка слайдов
		/// </summary>
		[HttpGet("{courseId}/{slideId}")]
		public async Task<ActionResult<ShortSlideInfo>> SlideInfo(Course course, Guid slideId)
		{
			var slide = course?.FindSlideById(slideId);
			if (slide == null)
			{
				var instructorNote = course?.FindInstructorNoteById(slideId);
				if (instructorNote != null && await CourseRolesRepo.HasUserAccessToAnyCourseAsync(User.GetUserId(), CourseRoleType.Instructor).ConfigureAwait(false))
					slide = instructorNote.Slide;
			}

			if(slide == null)
				return NotFound(new { status = "error", message = "Course or slide not found" });

			return BuildSlideInfo(course.Id, slide);
		}
	}
}