using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Database.Repos.CourseRoles;
using Database.Repos.Groups;
using Database.Repos.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Slides.Exercises;
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
		protected readonly ISlideCheckingsRepo slideCheckingsRepo;

		public SlidesController(ILogger logger, IWebCourseManager courseManager, UlearnDb db, IUsersRepo usersRepo, ICourseRolesRepo courseRolesRepo,
			IUserSolutionsRepo solutionsRepo, IUserQuizzesRepo userQuizzesRepo, IVisitsRepo visitsRepo, IGroupsRepo groupsRepo,
			SlideRenderer slideRenderer, ICoursesRepo coursesRepo, ISlideCheckingsRepo slideCheckingsRepo)
			: base(logger, courseManager, db, usersRepo)
		{
			this.coursesRepo = coursesRepo;
			this.courseRolesRepo = courseRolesRepo;
			this.solutionsRepo = solutionsRepo;
			this.userQuizzesRepo = userQuizzesRepo;
			this.visitsRepo = visitsRepo;
			this.groupsRepo = groupsRepo;
			this.slideRenderer = slideRenderer;
			this.slideCheckingsRepo = slideCheckingsRepo;
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

			List<UserExerciseSubmission> exerciseSubmissions = null;
			List<ExerciseCodeReviewComment> exerciseCodeReviewComments = null;
			if (slide is ExerciseSlide)
			{
				exerciseSubmissions = await solutionsRepo
					.GetAllSubmissionsByUser(course.Id, slideId, userId)
					.ToListAsync();
				exerciseCodeReviewComments = await slideCheckingsRepo.GetExerciseCodeReviewComments(course.Id, slideId, userId);
			}

			var slideRenderContext = new SlideRenderContext(course.Id, slide, baseUrl, !isInstructor,
				course.Settings.VideoAnnotationsGoogleDoc, Url, exerciseSubmissions, exerciseCodeReviewComments);

			return await slideRenderer.BuildSlideInfo(slideRenderContext, getSlideMaxScoreFunc, getGitEditLinkFunc);
		}
	}
}