using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using uLearn.Web.DataContexts;
using System.Web.Mvc;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class AnalyticsController : Controller
	{
		private readonly ULearnDb db = new ULearnDb();
		private readonly CourseManager courseManager;
		private readonly VisitersRepo visitersRepo = new VisitersRepo();
		private readonly SlideRateRepo slideRateRepo = new SlideRateRepo();
		private readonly UserSolutionsRepo userSolutionsRepo = new UserSolutionsRepo();
		private readonly SlideHintRepo slideHintRepo = new SlideHintRepo();
		private readonly UserQuizzesRepo userQuizzessRepo = new UserQuizzesRepo(); //TODO use in statistics


		public AnalyticsController() : this(CourseManager.AllCourses)
		{
		}

		public AnalyticsController(CourseManager courseManager)
		{
			this.courseManager = courseManager;
		}

		[Authorize]
		public ActionResult TotalStatistics(string courseId)
		{
			var model = CreateTotalStatistics(courseId);
			return View(model);
		}

		private AnalyticsTablePageModel CreateTotalStatistics(string courseId)
		{
			var course = courseManager.GetCourse(courseId);
			var tableInfo = CreateTotalStatisticsInfo(course);
			var model = new AnalyticsTablePageModel
			{
				TableInfo = tableInfo, 
				CourseId = courseId
			};
			return model;
		}

		private Dictionary<string, AnalyticsTableInfo> CreateTotalStatisticsInfo(Course course)
		{
			var tableInfo = new Dictionary<string, AnalyticsTableInfo>();
			foreach (var slide in course.Slides)
			{
				var exerciseSlide = (slide as ExerciseSlide);
				var isExercise = exerciseSlide != null;
				var hintsCountOnSlide = isExercise ? exerciseSlide.HintsHtml.Count() : 0;
				tableInfo.Add(slide.Info.Index + ". " + slide.Info.UnitName + ": " + slide.Title, new AnalyticsTableInfo
				{
					Rates = slideRateRepo.GetRates(slide.Id, course.Id),
					VisitersCount = visitersRepo.GetVisitersCount(slide.Id, course.Id),
					IsExercise = isExercise,
					SolversCount = isExercise ? userSolutionsRepo.GetAcceptedSolutionsCount(slide.Id, course.Id) : 0,
					TotalHintCount =  hintsCountOnSlide,
					HintUsedPercent = isExercise ? slideHintRepo.GetHintUsedPercent(slide.Id, course.Id, hintsCountOnSlide, db.Users.Count()) : 0 
				});
			}
			return tableInfo;
		}

		[Authorize]
		public ActionResult UsersStatistics(string courseId)
		{
			var course = courseManager.GetCourse(courseId);
			var model = CreateUsersStatisticsModel(course);
			return View(model);
		}

		private UsersStatsPageModel CreateUsersStatisticsModel(Course course)
		{
			return new UsersStatsPageModel
			{
				CourseId = course.Id,
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
		public ActionResult PersonalStatistics(string courseId, int slideIndex)
		{
			var course = courseManager.GetCourse(courseId);
			return View(CreatePersonalStaticticsModel(course));
		}

		private PersonalStatisticPageModel CreatePersonalStaticticsModel(Course course)
		{
			return new PersonalStatisticPageModel
			{
				CourseId = course.Id,
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
							UserRate = slideRateRepo.GetUserRate(course.Id, slide.Id, User.Identity.GetUserId()),
							HintsCountOnSlide = slide is ExerciseSlide ? (slide as ExerciseSlide).HintsHtml.Count() : 0,
							HintUsedPercent = slide is ExerciseSlide ? slideHintRepo.GetHintUsedPercentForUser(course.Id, slide.Id, User.Identity.GetUserId(), (slide as ExerciseSlide).HintsHtml.Count()) : 0
						})
						.ToArray()
				
			};
		}
	}
}