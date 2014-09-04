using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using uLearn.Quizes;
using uLearn.Web.DataContexts;
using System.Web.Mvc;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	[Authorize(Roles = LmsRoles.Instructor)]
	public class AnalyticsController : Controller
	{
		private readonly ULearnDb db = new ULearnDb();
		private readonly CourseManager courseManager;
		private readonly VisitersRepo visitersRepo = new VisitersRepo();
		private readonly SlideRateRepo slideRateRepo = new SlideRateRepo();
		private readonly UserSolutionsRepo userSolutionsRepo = new UserSolutionsRepo();
		private readonly SlideHintRepo slideHintRepo = new SlideHintRepo();
		private readonly UserQuizzesRepo userQuizzessRepo = new UserQuizzesRepo(); //TODO use in statistics


		public AnalyticsController()
			: this(WebCourseManager.Instance)
		{
		}

		public AnalyticsController(CourseManager courseManager)
		{
			this.courseManager = courseManager;
		}

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
						? (visitersCount == 0 ? 0 : (int)((double)userSolutionsRepo.GetAcceptedSolutionsCount(slide.Id, course.Id) / visitersCount) * 100)
						: isQuiz
							? (visitersCount == 0 ? 0 : (int)((double)userQuizzessRepo.GetSubmitQuizCount(slide.Id, course.Id) / visitersCount) * 100)
							: 0,
					SuccessQuizPercentage = isQuiz ? userQuizzessRepo.GetAverageStatistics(slide.Id, course.Id) : 0,
					TotalHintCount = hintsCountOnSlide,
					HintUsedPercent = isExercise ? slideHintRepo.GetHintUsedPercent(slide.Id, course.Id, hintsCountOnSlide, db.Users.Count()) : 0
				});
			}
			return tableInfo;
		}

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
					ans[userRates.Key][rate.Key] = (int)(100 * (double)rate.Value / slideCountInUnit[rate.Key]);
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
				var user = db.Users.Find(userSolution.UserId);
				if (user == null) continue;
				var userName = user.UserName;
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
				var user = db.Users.Find(quiz.UserId);
				if (user == null) continue;
				var userName = user.UserName;
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
			var model = new UnitStatisticPageModel
			{
				CourseId = courseId,
				UnitName = unitName,
				Slides = slides,
				SlideRateStats = GetSlideRateStats(course, slides),
			};
			return View(model);
		}
		
		public ActionResult SlideRatings(string courseId, string unitName)
		{
			var course = courseManager.GetCourse(courseId);
			var slides = course.Slides
				.Where(s => s.Info.UnitName == unitName).ToArray();
			var model = GetSlideRateStats(course, slides);
			return PartialView(model);
		}

		public ActionResult DailyStatistics(string courseId, string unitName)
		{
			var course = courseManager.GetCourse(courseId);
			var slides = course.Slides
				.Where(s => s.Info.UnitName == unitName).ToArray();
			var model = GetDailyStatistics(course, slides);
			return PartialView(model);
		}


		public ActionResult UsersProgress(string courseId, string unitName)
		{
			var course = courseManager.GetCourse(courseId);
			var slides = course.Slides
				.Where(s => s.Info.UnitName == unitName).ToArray();
			var users = GetUserInfos(course, slides).OrderByDescending(GetRating).ToArray();
			return PartialView(new UserProgressViewModel{Slides = slides, Users = users});
		}

		private DailyStatistics[] GetDailyStatistics(Course course, Slide[] slides)
		{
			var slideIds = slides.Select(s => s.Id).ToArray();
			var courseId = course.Id;
			var lastDay = DateTime.Now.Date.Add(new TimeSpan(0, 23, 59, 59, 999));
			var firstDay = lastDay.Date.AddDays(-14);
			var tasks = GetTasksSolvedStats(slideIds, firstDay, lastDay);
			var quizes = GetQuizPassedStats(slideIds, firstDay, lastDay);
			var visits = GetSlidesVisitedStats(slideIds, firstDay, lastDay);
			var result =
				Enumerable.Range(0, 14)
				.Select(diff => lastDay.AddDays(-diff).Date)
				.Select(date => new DailyStatistics
				{
					Day = date, 
					TasksSolved = tasks.Get(date, 0) + quizes.Get(date, 0),
					SlidesVisited = visits.Get(date, 0)
				})
				.ToArray();
			return result;
		}

		private Dictionary<DateTime, int> GetTasksSolvedStats(IEnumerable<string> slideIds, DateTime firstDay, DateTime lastDay)
		{
			var q = from s in db.UserSolutions
					where slideIds.Contains(s.SlideId) && s.Timestamp > firstDay && s.Timestamp <= lastDay && s.IsRightAnswer
					group s by DbFunctions.TruncateTime(s.Timestamp)
						into day
						select new { day.Key, count = day.Select(d => new { d.UserId, d.SlideId }).Distinct().Count() };
			return q.ToDictionary(d => d.Key.Value, d => d.count);
		}

		private Dictionary<DateTime, int> GetQuizPassedStats(IEnumerable<string> slideIds, DateTime firstDay, DateTime lastDay)
		{
			var query = from q in db.UserQuizzes
						where slideIds.Contains(q.SlideId) && q.Timestamp > firstDay && q.Timestamp <= lastDay
						group q by DbFunctions.TruncateTime(q.Timestamp)
							into day
							select new { day.Key, count = day.Select(d => new { d.UserId, d.SlideId }).Distinct().Count() };
			return query.ToDictionary(d => d.Key.Value, d => d.count);
		}
		
		private Dictionary<DateTime, int> GetSlidesVisitedStats(IEnumerable<string> slideIds, DateTime firstDay, DateTime lastDay)
		{
			var query = from v in db.Visiters
						where slideIds.Contains(v.SlideId) && v.Timestamp > firstDay && v.Timestamp <= lastDay
						group v by DbFunctions.TruncateTime(v.Timestamp)
							into day
							select new { day.Key, count = day.Select(d => new { d.UserId, d.SlideId }).Distinct().Count() };
			return query.ToDictionary(d => d.Key.Value, d => d.count);
		}

		private SlideRateStats[] GetSlideRateStats(Course course, IEnumerable<Slide> slides)
		{
			var courseId = course.Id;
			var rates =
				(from rate in db.SlideRates
				where rate.CourseId == courseId
				group rate by new {rate.SlideId, rate.Rate}
				into slideRate
				select new
				{
					slideRate.Key.SlideId,
					slideRate.Key.Rate,
					count = slideRate.Count()
				})
				.ToLookup(r => r.SlideId);
			return slides.Select(s => new SlideRateStats
			{
				SlideId = s.Id,
				SlideTitle = s.Title,
				NotUnderstand = rates[s.Id].Where(r => r.Rate == SlideRates.NotUnderstand).Sum(r => r.count),
				Good = rates[s.Id].Where(r => r.Rate == SlideRates.Good).Sum(r => r.count),
				Trivial = rates[s.Id].Where(r => r.Rate == SlideRates.Trivial).Sum(r => r.count),
			}).ToArray();
		}

		private double GetRating(UserInfo user)
		{
			return
				user.SlidesSlideInfo.Sum(
					s => 
						(s.IsVisited ? 1 : 0) 
						+ (s.IsExerciseSolved ? 1 : 0) 
						+ (s.IsQuizPassed ? s.QuizPercentage/100.0 : 0));
		}

		private IEnumerable<UserInfo> GetUserInfos(Course course, Slide[] slides)
		{
			var slideIds = slides.Select(s => s.Id).ToArray();
			var courseId = course.Id;
			var dataQuery =
				from v in db.Visiters
				where
					v.CourseId == courseId
					&& slideIds.Contains(v.SlideId)
				join s in db.UserSolutions on new {v.SlideId, v.UserId} equals new {s.SlideId, s.UserId} into sInner
				from ss in sInner.DefaultIfEmpty()
				join q in db.UserQuizzes on new {v.SlideId, v.UserId} equals new {q.SlideId, q.UserId} into qInner
				from qq in qInner.DefaultIfEmpty()
				join u in db.Users on v.UserId equals u.Id into users
				from uu in users
				select new
				{
					v.UserId,
					uu.UserName,
					uu.GroupName,
					v.SlideId,
					SolvedExercise = ss != null && ss.IsRightAnswer,
					IsRightQuiz = qq != null && qq.IsRightQuizBlock
				};
//			Debug.Write(dataQuery.ToString());

			var res = from v in dataQuery.ToList()
					  group v by new { v.UserId, v.UserName, v.GroupName }
						  into u
						  select FillUserInfo(
							  new UserInfo
							  {
								  UserId = u.Key.UserId,
								  UserName = u.Key.UserName,
								  UserGroup = u.Key.GroupName
							  },
							  slides,
							  u.Select(d => Tuple.Create(d.SlideId, d.SolvedExercise, d.IsRightQuiz))
							  );
			return res;
		}

		private UserInfo FillUserInfo(UserInfo userInfo, IEnumerable<Slide> slides, IEnumerable<Tuple<string, bool, bool>> slideExerciseQuiz)
		{
			var lookup = slideExerciseQuiz.ToLookup(tuple => tuple.Item1);
			userInfo.SlidesSlideInfo = slides
				.Select(
					slide => new
					{
						slide,
						quizAnswers = lookup[slide.Id].Select(t => t.Item3),
						solutions = lookup[slide.Id].Select(t => t.Item2)
					}
				)
				.Select(
					slide => new UserSlideInfo
					{
						AttemptsCount = slide.solutions.Count(),
						IsExerciseSolved = slide.solutions.Any(sol => sol),
						IsQuizPassed = slide.quizAnswers.Any(),
						QuizPercentage = slide.quizAnswers.Any() ? (double)slide.quizAnswers.Count(q => q) / slide.quizAnswers.Count() : 0.0,
						IsVisited = slide.solutions.Any() // Если была запись в visited
					}).ToArray();
			return userInfo;
		}

		[HttpPost]
		public async Task<ActionResult> AddUserGroup(string groupName, string userId)
		{
			db.Users.First(x => x.Id == userId).GroupName = groupName;
			await db.SaveChangesAsync();
			return null;
		}
	}

	public class UserProgressViewModel
	{
		public UserInfo[] Users;
		public Slide[] Slides;
	}

}