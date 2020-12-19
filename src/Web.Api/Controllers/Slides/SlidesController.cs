using System;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Database.Repos.CourseRoles;
using Database.Repos.Groups;
using Database.Repos.Users;
using Microsoft.AspNetCore.Mvc;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Controllers.Slides
{
	[Route("/slides")]
	public class SlidesController : BaseController
	{
		protected readonly ICoursesRepo coursesRepo;
		protected readonly ICourseRolesRepo courseRolesRepo;
		protected readonly IUserSolutionsRepo solutionsRepo;
		protected readonly IUserQuizzesRepo userQuizzesRepo;
		protected readonly IVisitsRepo visitsRepo;
		protected readonly IGroupsRepo groupsRepo;
		protected readonly SlideRenderer slideRenderer;

		public SlidesController(IWebCourseManager courseManager, UlearnDb db, IUsersRepo usersRepo, ICourseRolesRepo courseRolesRepo,
			IUserSolutionsRepo solutionsRepo, IUserQuizzesRepo userQuizzesRepo, IVisitsRepo visitsRepo, IGroupsRepo groupsRepo,
			SlideRenderer slideRenderer, ICoursesRepo coursesRepo)
			: base(courseManager, db, usersRepo)
		{
			this.coursesRepo = coursesRepo;
			this.courseRolesRepo = courseRolesRepo;
			this.solutionsRepo = solutionsRepo;
			this.userQuizzesRepo = userQuizzesRepo;
			this.visitsRepo = visitsRepo;
			this.groupsRepo = groupsRepo;
			this.slideRenderer = slideRenderer;
		}

		/// <summary>
		/// Информация о слайде
		/// </summary>
		[HttpGet("{courseId}/{slideId}")]
		public async Task<ActionResult<ApiSlideInfo>> SlideInfo([FromRoute] Course course, [FromRoute] Guid slideId)
		{
			var isInstructor = await courseRolesRepo.HasUserAccessToAnyCourseAsync(User.GetUserId(), CourseRoleType.Instructor).ConfigureAwait(false);
			var slide = course?.FindSlideById(slideId, isInstructor);
			if (slide == null)
			{
				var instructorNote = course?.FindInstructorNoteById(slideId);
				if (instructorNote != null && isInstructor)
					slide = instructorNote.Slide;
			}

			if (slide == null)
				return NotFound(new { status = "error", message = "Course or slide not found" });

			var userId = User.GetUserId();
			var getSlideMaxScoreFunc = await BuildGetSlideMaxScoreFunc(solutionsRepo, userQuizzesRepo, visitsRepo, groupsRepo, course, userId);
			var getGitEditLinkFunc = await BuildGetGitEditLinkFunc(userId, course, courseRolesRepo, coursesRepo);
			var baseUrl = CourseUnitUtils.GetDirectoryRelativeWebPath(slide.Info.SlideFile);

			var slideRenderContext = new SlideRenderContext(course.Id, slide, UserId, baseUrl, !isInstructor,
				course.Settings.VideoAnnotationsGoogleDoc, Url);

			return await slideRenderer.BuildSlideInfo(slideRenderContext, getSlideMaxScoreFunc, getGitEditLinkFunc);
		}
	}
}