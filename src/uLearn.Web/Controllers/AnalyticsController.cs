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
		private readonly AnalyticsTableRepo analyticsTable = new AnalyticsTableRepo();
		private readonly CourseManager courseManager;

		public AnalyticsController() : this(CourseManager.AllCourses)
		{
		}

		public AnalyticsController(CourseManager courseManager)
		{
			this.courseManager = courseManager;
		}

		[Authorize]
		public ActionResult TableAnalytics(string courseId, string slideIndex)
		{
			int slideIndexInt;
			int.TryParse(slideIndex, out slideIndexInt);
			var model = CreateAnalyticsTable(courseId, CreateCoursePageModel(courseId, slideIndexInt));
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
				var key = analyticsTable.CreateKey(course.Title, slide.Info.UnitName, slide.Title);
				var marks = analyticsTable.GetMarks(key);
				var isExercise = (slide as ExerciseSlide) != null;
				tableInfo.Add(slide.Info.UnitName + ": " + slide.Title, new AnalyticsTableInfo
				{
					Marks = marks,
					SolversCount = analyticsTable.GetSolversCount(key),
					VisitersCount = analyticsTable.GetVisitersCount(key),
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
		public ActionResult UsersStatistics(string courseId, string slideIndex)
		{
			int slideIndexInt;
			int.TryParse(slideIndex, out slideIndexInt);
			var course = courseManager.GetCourse(courseId);
			var model = CreateUsersStatsModel(course, CreateCoursePageModel(courseId, slideIndexInt), slideIndexInt);
			return View(model);
		}

		private UsersStatsPageModel CreateUsersStatsModel(Course course, CoursePageModel coursePageModel, int slideIndex)
		{
			return new UsersStatsPageModel
			{
				CoursePageModel = coursePageModel,
				UserStats = analyticsTable.CreateUsersStats(course, slideIndex)
			};
		}

		[Authorize]
		public ActionResult PersonalStatistics(string courseId, string slideIndex)
		{
			int slideIndexInt;
			int.TryParse(slideIndex, out slideIndexInt);
			var coursePageModel = CreateCoursePageModel(courseId, slideIndexInt);
			var course = courseManager.GetCourse(courseId);
			return View(CreatePersonalStaticticsModel(coursePageModel, User.Identity.GetUserId(), course));
		}

		private PersonalStatisticPageModel CreatePersonalStaticticsModel(CoursePageModel coursePageModel, string userId, Course course)
		{
			return new PersonalStatisticPageModel
			{
				CoursePageModel = coursePageModel,
				PersonalStatistics = analyticsTable.CreatePersonalStatistics(userId, course).Result
			};
		}
	}
}