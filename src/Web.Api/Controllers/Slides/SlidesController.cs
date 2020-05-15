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

namespace Ulearn.Web.Api.Controllers.Slides
{
	[Route("/slides")]
	public class SlidesController : BaseController
	{
		protected readonly ICourseRolesRepo CourseRolesRepo;
		protected readonly IUserSolutionsRepo SolutionsRepo;
		protected readonly IUserQuizzesRepo UserQuizzesRepo;
		protected readonly IVisitsRepo VisitsRepo;
		protected readonly IGroupsRepo GroupsRepo;
		protected readonly SlideRenderer SlideRenderer;

		public SlidesController(ILogger logger, IWebCourseManager courseManager, UlearnDb db, IUsersRepo usersRepo, ICourseRolesRepo courseRolesRepo,
			IUserSolutionsRepo solutionsRepo, IUserQuizzesRepo userQuizzesRepo, IVisitsRepo visitsRepo, IGroupsRepo groupsRepo,
			SlideRenderer slideRenderer)
			: base(logger, courseManager, db, usersRepo)
		{
			CourseRolesRepo = courseRolesRepo;
			SolutionsRepo = solutionsRepo;
			UserQuizzesRepo = userQuizzesRepo;
			VisitsRepo = visitsRepo;
			GroupsRepo = groupsRepo;
			SlideRenderer = slideRenderer;
		}

		/// <summary>
		/// Информация о слайде
		/// </summary>
		[HttpGet("{courseId}/{slideId}")]
		public async Task<ActionResult<ApiSlideInfo>> SlideInfo([FromRoute]Course course, [FromRoute]Guid slideId)
		{
			var slide = course?.FindSlideById(slideId);
			var isInstructor = await CourseRolesRepo.HasUserAccessToAnyCourseAsync(User.GetUserId(), CourseRoleType.Instructor).ConfigureAwait(false);
			if (slide == null)
			{
				var instructorNote = course?.FindInstructorNoteById(slideId);
				if (instructorNote != null && isInstructor)
					slide = instructorNote.Slide;
			}

			if (slide == null)
				return NotFound(new { status = "error", message = "Course or slide not found" });

			var getSlideMaxScoreFunc = await BuildGetSlideMaxScoreFunc(SolutionsRepo, UserQuizzesRepo, VisitsRepo, GroupsRepo, course, User.GetUserId());
			var baseUrl = CourseUnitUtils.GetDirectoryRelativeWebPath(slide.Info.SlideFile);
			var slideRenderContext = new SlideRenderContext(course.Id, slide, baseUrl, !isInstructor,
				course.Settings.VideoAnnotationsGoogleDoc, Url);
			return await SlideRenderer.BuildSlideInfo(slideRenderContext, getSlideMaxScoreFunc);
		}
	}
}