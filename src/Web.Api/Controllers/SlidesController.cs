using System;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Database.Repos.CourseRoles;
using Database.Repos.Groups;
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
		protected readonly IUserSolutionsRepo SolutionsRepo;
		protected readonly IUserQuizzesRepo UserQuizzesRepo;
		protected readonly IVisitsRepo VisitsRepo;
		protected readonly IGroupsRepo GroupsRepo;

		public SlidesController(ILogger logger, IWebCourseManager courseManager, UlearnDb db, IUsersRepo usersRepo, ICourseRolesRepo courseRolesRepo,
			IUserSolutionsRepo solutionsRepo, IUserQuizzesRepo userQuizzesRepo, IVisitsRepo visitsRepo, IGroupsRepo groupsRepo)
			: base(logger, courseManager, db, usersRepo)
		{
			CourseRolesRepo = courseRolesRepo;
			SolutionsRepo = solutionsRepo;
			UserQuizzesRepo = userQuizzesRepo;
			VisitsRepo = visitsRepo;
			GroupsRepo = groupsRepo;
		}

		/// <summary>
		/// Информация о слайде, достаточная для отображения списка слайдов
		/// </summary>
		[HttpGet("{courseId}/{slideId}")]
		public async Task<ActionResult<ShortSlideInfo>> SlideInfo([FromQuery]Course course, [FromQuery]Guid slideId)
		{
			var slide = course?.FindSlideById(slideId);
			if (slide == null)
			{
				var instructorNote = course?.FindInstructorNoteById(slideId);
				if (instructorNote != null && await CourseRolesRepo.HasUserAccessToAnyCourseAsync(User.GetUserId(), CourseRoleType.Instructor).ConfigureAwait(false))
					slide = instructorNote.Slide;
			}

			if (slide == null)
				return NotFound(new { status = "error", message = "Course or slide not found" });

			var getSlideMaxScoreFunc = await BuildGetSlideMaxScoreFunc(SolutionsRepo, UserQuizzesRepo, VisitsRepo, GroupsRepo, course, User.GetUserId());
			return BuildSlideInfo(course.Id, slide, getSlideMaxScoreFunc);
		}
	}
}