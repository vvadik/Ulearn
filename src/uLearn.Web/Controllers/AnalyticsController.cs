using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using uLearn.Quizes;
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
				var quizSlide = (slide as QuizSlide);
				var isExercise = exerciseSlide != null;
				var isQuiz = quizSlide != null;
				var hintsCountOnSlide = isExercise ? exerciseSlide.HintsHtml.Count() : 0;
				var visitersCount = visitersRepo.GetVisitersCount(slide.Id, course.Id);
				tableInfo.Add(slide.Index + ". " + slide.Info.UnitName + ": " + slide.Title, new AnalyticsTableInfo
				{
					Rates = slideRateRepo.GetRates(slide.Id, course.Id),
					VisitersCount = visitersCount,
					IsExercise = isExercise,
					IsQuiz = isQuiz,
					SolversPercent = isExercise 
						? (visitersCount == 0 ? 0 : (int)((double)userSolutionsRepo.GetAcceptedSolutionsCount(slide.Id, course.Id)/visitersCount)*100)
						: isQuiz
							? (visitersCount == 0 ? 0 : (int)((double)userQuizzessRepo.GetSubmitQuizCount(slide.Id, course.Id) / visitersCount) * 100)
							: 0,
					SuccessQuizPercentage = isQuiz ? userQuizzessRepo.GetAverageStatistics(slide.Id, course.Id) : 0,
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
			foreach (var slide in course.Slides.Where(x => x is ExerciseSlide || x is QuizSlide))
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
			foreach (var quiz in db.UserQuizzes)
			{
				if (!unitNames.ContainsKey(quiz.SlideId)) //пока в старой базе есть старые записи с неправильными ID
					continue;
				var userName = db.Users.Find(quiz.UserId).UserName;
				if (!acceptedSolutionsForUsers.ContainsKey(userName))
					acceptedSolutionsForUsers[userName] = new HashSet<string>();
				if (acceptedSolutionsForUsers[userName].Contains(quiz.SlideId))
					continue;
				acceptedSolutionsForUsers[userName].Add(quiz.SlideId);
				if (!ans[userName].ContainsKey(unitNames[quiz.SlideId]))
					ans[userName][unitNames[quiz.SlideId]] = 0;
				ans[userName][unitNames[quiz.SlideId]]++;
			}
		}

		[Authorize]
		public ActionResult PersonalStatistics(string courseId)
		{
			var course = courseManager.GetCourse(courseId);
			return View(CreatePersonalStaticticsModel(course));
		}

		private PersonalStatisticPageModel CreatePersonalStaticticsModel(Course course)
		{
			var userId = User.Identity.GetUserId();
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
							IsQuiz = slide is QuizSlide,
							IsSolved = userSolutionsRepo.IsUserPassedTask(course.Id, slide.Id, userId) || userQuizzessRepo.IsQuizSlidePassed(course.Id, userId, slide.Id),
							IsVisited = visitersRepo.IsUserVisit(course.Id, slide.Id, userId),
							UserRate = slideRateRepo.GetUserRate(course.Id, slide.Id, userId),
							HintsCountOnSlide = slide is ExerciseSlide ? (slide as ExerciseSlide).HintsHtml.Count() : 0,
							HintUsedPercent = slide is ExerciseSlide ? slideHintRepo.GetHintUsedPercentForUser(course.Id, slide.Id, userId, (slide as ExerciseSlide).HintsHtml.Count()) : 0
						})
						.ToArray()
			};
		}

		public ActionResult UnitStatistics(string courseId, string unitName)
		{
			var course = courseManager.GetCourse(courseId);
			var slides = course.Slides
				.Where(s => s.Info.UnitName == unitName).ToArray();
			var unitStatisticPageModel = new UnitStatisticPageModel
			{
				CourseId = courseId,
				Slides = slides,
				Table = new Dictionary<string, UserInfo>()
			};
			var slideIdToSlideIndex = new Dictionary<string, int>();
			for (var i = 0; i < slides.Length; i++)
				slideIdToSlideIndex[slides[i].Id] = i;
			foreach (var user in db.Users)
			{
				unitStatisticPageModel.Table[user.UserName] = new UserInfo();
				unitStatisticPageModel.Table[user.UserName].UserGroup = user.GroupName ?? "";
				unitStatisticPageModel.Table[user.UserName].SlidesInfo = new UserInfoInSlide[slides.Length];
				for (var i = 0; i < slides.Length; i++)
					unitStatisticPageModel.Table[user.UserName].SlidesInfo[i] = new UserInfoInSlide();
			}
			foreach (var userSolutions in db.UserSolutions.Where(x => x.CourseId == courseId).GroupBy(x => x.UserId))
			{
				var name = db.Users.Find(userSolutions.Key).UserName;
				foreach (var slideGroup in userSolutions.Where(x => slideIdToSlideIndex.ContainsKey(x.SlideId)).GroupBy(x => x.SlideId))
				{
					var info = unitStatisticPageModel.Table[name].SlidesInfo[slideIdToSlideIndex[slideGroup.Key]];
					info.AttemptsNumber = slideGroup.Count();
					info.IsExerciseSolved = slideGroup.Any(x => x.IsRightAnswer);
				}
			}
			foreach (var user in db.Users)
			{
				for (var i = 0; i < slides.Length; i++)
				{
					unitStatisticPageModel.Table[user.UserName].SlidesInfo[i].IsVisited = 
						visitersRepo.IsUserVisit(courseId, slides[i].Id, user.Id);
				}
			}
			foreach (var user in db.Users)
			{
				for (var i = 0; i < slides.Length; i++)
				{
					if (!(slides[i] is QuizSlide)) continue;
					unitStatisticPageModel.Table[user.UserName].SlidesInfo[i].IsQuizPassed = 
						userQuizzessRepo.IsQuizSlidePassed(courseId, user.Id, slides[i].Id);
					unitStatisticPageModel.Table[user.UserName].SlidesInfo[i].QuizSuccessful =
						userQuizzessRepo.GetQuizSuccessful(courseId, slides[i].Id, user.Id);
				}
			}
			return View(unitStatisticPageModel);
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> AddUserGroup(string groupName, string userName)
		{
			db.Users.First(x => x.UserName == userName).GroupName = groupName;
			await db.SaveChangesAsync();
			return null;
		}
	}
}