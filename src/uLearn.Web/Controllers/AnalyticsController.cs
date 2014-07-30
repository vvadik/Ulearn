using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Security;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using uLearn.Web.DataContexts;
using System.Web.Mvc;
using uLearn.Web.Models;
using uLearn.Web.Ideone;

namespace uLearn.Web.Controllers
{
	public class AnalyticsController : Controller
	{
		private readonly ULearnDb db = new ULearnDb();
		private readonly CourseManager courseManager;
		private readonly VisitersRepo visitersRepo = new VisitersRepo();
		private readonly SlideRateRepo slideRateRepo = new SlideRateRepo();
		private readonly UserSolutionsRepo userSolutionsRepo = new UserSolutionsRepo();


		public AnalyticsController()
			: this(CourseManager.AllCourses)
		{
		}

		public AnalyticsController(CourseManager courseManager)
		{
			this.courseManager = courseManager;
		}

		[Authorize]
		public ActionResult TotalStatistics(string courseId, int slideIndex)
		{
			var model = CreateTotalStatistics(courseId, CreateCoursePageModel(courseId, slideIndex));
			return View(model);
		}

		private AnalyticsTablePageModel CreateTotalStatistics(string courseId, CoursePageModel coursePageModel)
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
		public ActionResult UsersStatistics(string courseId, int slideIndex)
		{
			var course = courseManager.GetCourse(courseId);
			var model = CreateUsersStatisticsModel(course, CreateCoursePageModel(courseId, slideIndex));
			return View(model);
		}

		private UsersStatsPageModel CreateUsersStatisticsModel(Course course, CoursePageModel coursePageModel)
		{
			return new UsersStatsPageModel
			{
				CoursePageModel = coursePageModel,
				UserStats = CreateUserStats(course),
				UnitNamesInOrdered = course.Slides.GroupBy(x => x.Info.UnitName).Select(x => x.Key).ToList()
			};
		}

		private Dictionary<string, SortedDictionary<string, int>> CreateUserStats(Course course)
		{
			var users = db.Users.ToList();
			var ans = new Dictionary<string, SortedDictionary<string, int>>();
			foreach (var user in users)
				ans[user.UserName] = new SortedDictionary<string, int>();
			var acceptedSolutionsForUsers = new Dictionary<string, HashSet<string>>();
			var unitNames = new Dictionary<string, string>();
			foreach (var slide in course.Slides)
				unitNames[slide.Id] = slide.Info.UnitName;
			var slideCountInUnit = new Dictionary<string, int>();
			FillCapacityOfUnits(course, slideCountInUnit);
			FillTheTable(unitNames, acceptedSolutionsForUsers, ans);
			InitialEmptyUnit(slideCountInUnit, ans);
			CalculatePercent(ans, slideCountInUnit);
			return ans;
		}

		private static void FillCapacityOfUnits(Course course, Dictionary<string, int> slideCountInUnit)
		{
			foreach (var slide in course.Slides.OfType<ExerciseSlide>())
			{
				if (!slideCountInUnit.ContainsKey(slide.Info.UnitName))
					slideCountInUnit[slide.Info.UnitName] = 0;
				slideCountInUnit[slide.Info.UnitName]++;
			}
		}

		private static void CalculatePercent(Dictionary<string, SortedDictionary<string, int>> ans, Dictionary<string, int> slideCountInUnit)
		{
			foreach (var userRates in ans.ToList())
				foreach (var rate in userRates.Value.ToList())
					ans[userRates.Key][rate.Key] = (int) (100*(double) rate.Value/slideCountInUnit[rate.Key]);
		}

		private static void InitialEmptyUnit(Dictionary<string, int> slideCountInUnit, Dictionary<string, SortedDictionary<string, int>> ans)
		{
			foreach (var unit in slideCountInUnit.Keys)
			{
				var unitName = unit;
				foreach (var user in ans.Keys.Where(user => !ans[user].ContainsKey(unitName)))
					ans[user].Add(unit, 0);
			}
		}

		private void FillTheTable(Dictionary<string, string> unitNames, Dictionary<string, HashSet<string>> acceptedSolutionsForUsers, Dictionary<string, SortedDictionary<string, int>> ans)
		{
			foreach (var userSolution in db.UserSolutions.Where(x => x.IsRightAnswer))
			{
				if (!unitNames.ContainsKey(userSolution.SlideId)) //пока в старой базе есть старые записи с неправильными ID
					continue;
				var userName = db.Users.Find(userSolution.UserId).UserName;
				if (!acceptedSolutionsForUsers.ContainsKey(userName))
					acceptedSolutionsForUsers[userName] = new HashSet<string>();
				if (acceptedSolutionsForUsers[userName].Contains(userSolution.SlideId))
					continue;
				acceptedSolutionsForUsers[userName].Add(userSolution.SlideId);
				if (!ans[userName].ContainsKey(unitNames[userSolution.SlideId]))
					ans[userName][unitNames[userSolution.SlideId]] = 0;
				ans[userName][unitNames[userSolution.SlideId]]++;
			}
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
				PersonalStatistics =
					course.Slides
						.Select((slide, slideIndex) => new PersonalStatisticsInSlide
		{
							UnitName = slide.Info.UnitName,
							SlideTitle = slide.Title,
							SlideIndex = slideIndex,
					IsExercise = slide is ExerciseSlide,
					IsSolved = userSolutionsRepo.IsUserPassedTask(course.Id, slide.Id, User.Identity.GetUserId()),
					IsVisited = visitersRepo.IsUserVisit(course.Id, slide.Id, User.Identity.GetUserId()),
					UserMark = slideRateRepo.GetUserRate(course.Id, slide.Id, User.Identity.GetUserId())
						})
						.ToArray()
				};
			}
		}
}