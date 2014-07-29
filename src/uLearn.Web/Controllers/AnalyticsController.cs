using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using uLearn.Web.DataContexts;
using System.Web.Mvc;
using uLearn.Web.Models;
using uLearn.Web.Ideone;

namespace uLearn.Web.Controllers
{
	public class AnalyticsController : Controller
	{
		private readonly CourseManager courseManager;
		private readonly VisitersRepo visitersRepo = new VisitersRepo();
		private readonly SlideRateRepo slideRateRepo = new SlideRateRepo();
		private readonly UserSolutionsRepo userSolutionsRepo = new UserSolutionsRepo();


		public AnalyticsController() : this(CourseManager.AllCourses)
		{
		}

		public AnalyticsController(CourseManager courseManager)
		{
			this.courseManager = courseManager;
		}

		[Authorize]
		public ActionResult TableAnalytics(string courseId, int slideIndex)
		{
			var model = CreateAnalyticsTable(courseId, CreateCoursePageModel(courseId, slideIndex));
			return View(model);
		}

		private AnalyticsTablePageModel CreateAnalyticsTable(string courseId, CoursePageModel coursePageModel)
		{
			var course = courseManager.GetCourse(courseId);
			var tableInfo = CreateTableInfo(course);
			var model = new AnalyticsTablePageModel {Course = course, TableInfo = tableInfo, CoursePageModel = coursePageModel};
			return model;
		}

		private Dictionary<string, AnalyticsTableInfo> CreateTableInfo(Course course)
		{
			var tableInfo = new Dictionary<string, AnalyticsTableInfo>();
			foreach (var slide in course.Slides)
			{
				var isExercise = (slide is ExerciseSlide);
				tableInfo.Add(slide.Info.UnitName + ": " + slide.Title, new AnalyticsTableInfo
				{
					Marks = slideRateRepo.GetRates(slide.Id, course.Id),
					SolversCount = userSolutionsRepo.GetAcceptedSolutionsCount(slide.Id, course.Id),
					VisitersCount = visitersRepo.GetVisitersCount(slide.Id, course.Id),
					IsExercise = isExercise
				});
			}
			return tableInfo;
		}

		private CoursePageModel CreateCoursePageModel(string courseId, int slideIndex)
		{
			Course course = courseManager.GetCourse(courseId);
			var model = new CoursePageModel
			{
				Course = course,
				SlideIndex = slideIndex,
				Slide = course.Slides[slideIndex],
				NextSlideIndex = slideIndex + 1,
				PrevSlideIndex = slideIndex - 1,
				IsPassedTask = false,
				LatestAcceptedSolution = null
			};
			return model;
		}

		[Authorize]
		public ActionResult UsersStatistics(string courseId, string slideIndex) //while not using
		{
			int slideIndexInt;
			int.TryParse(slideIndex, out slideIndexInt);
			var course = courseManager.GetCourse(courseId);
			var model = CreateUsersStatsModel(course, CreateCoursePageModel(courseId, slideIndexInt), slideIndexInt);
			return View(model);
		}

		private UsersStatsPageModel CreateUsersStatsModel(Course course, CoursePageModel coursePageModel, int slideIndex) //while not using
		{
			return new UsersStatsPageModel
			{
				CoursePageModel = coursePageModel,
				UserStats = null
			};
		}

		[Authorize]
		public ActionResult PersonalStatistics(string courseId, string slideIndex)
		{
			int slideIndexInt;
			int.TryParse(slideIndex, out slideIndexInt);
			var coursePageModel = CreateCoursePageModel(courseId, slideIndexInt);
			var course = courseManager.GetCourse(courseId);
			return View(CreatePersonalStaticticsModel(coursePageModel, course));
		}

		private PersonalStatisticPageModel CreatePersonalStaticticsModel(CoursePageModel coursePageModel, Course course)
		{
			return new PersonalStatisticPageModel
			{
				CoursePageModel = coursePageModel,
				PersonalStatistics = CreatePersonalStatistic(course)
			};
		}

		private Dictionary<string, PersonalStatisticsInSlide> CreatePersonalStatistic(Course course)
		{
			var ans = new Dictionary<string, PersonalStatisticsInSlide>();
			foreach (var slide in course.Slides)
			{
				ans[slide.Info.UnitName + ": " + slide.Title] = new PersonalStatisticsInSlide
				{
					IsNotExercise = !(slide is ExerciseSlide),
					IsSolved = userSolutionsRepo.IsUserPassedTask(course.Id, slide.Id, User.Identity.GetUserId()),
					IsVisited = visitersRepo.IsUserVisit(course.Id, slide.Id, User.Identity.GetUserId()),
					UserMark = slideRateRepo.GetUserRate(course.Id, slide.Id, User.Identity.GetUserId())
				};
			}
			return ans;
		}
	}
}