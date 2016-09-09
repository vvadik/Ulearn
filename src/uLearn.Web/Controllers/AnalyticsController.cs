using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using uLearn.Quizes;
using uLearn.Web.DataContexts;
using uLearn.Web.FilterAttributes;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
	public class AnalyticsController : Controller
	{
		private readonly ULearnDb db = new ULearnDb();
		private readonly CourseManager courseManager;
		private readonly VisitsRepo visitsRepo = new VisitsRepo();
		private readonly SlideRateRepo slideRateRepo = new SlideRateRepo();
		private readonly UserSolutionsRepo userSolutionsRepo = new UserSolutionsRepo();
		private readonly SlideHintRepo slideHintRepo = new SlideHintRepo();
		private readonly UserQuizzesRepo userQuizzesRepo = new UserQuizzesRepo();
		private readonly CommentsRepo commentsRepo = new CommentsRepo();
		private readonly GroupsRepo groupsRepo = new GroupsRepo();
		private readonly SlideCheckingsRepo slideCheckingsRepo = new SlideCheckingsRepo();

		public AnalyticsController()
			: this(WebCourseManager.Instance)
		{
		}

		public AnalyticsController(CourseManager courseManager)
		{
			this.courseManager = courseManager;
		}

		private ActionResult TotalStatistics(string courseId)
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
				var exerciseSlide = slide as ExerciseSlide;
				var quizSlide = slide as QuizSlide;
				var isExercise = exerciseSlide != null;
				var isQuiz = quizSlide != null;
				var hintsCountOnSlide = isExercise ? exerciseSlide.Exercise.HintsMd.Count : 0;
				var visitersCount = visitsRepo.GetVisitsCount(slide.Id, course.Id);
				tableInfo.Add(slide.Index + ". " + slide.Info.UnitName + ": " + slide.Title, new AnalyticsTableInfo
				{
					Rates = slideRateRepo.GetRates(slide.Id, course.Id),
					VisitersCount = visitersCount,
					IsExercise = isExercise,
					IsQuiz = isQuiz,
					SolversPercent = isExercise
						? (visitersCount == 0 ? 0 : (int)((double)userSolutionsRepo.GetAcceptedSolutionsCount(slide.Id, course.Id) / visitersCount) * 100)
						: isQuiz
							? (visitersCount == 0 ? 0 : (int)((double)userQuizzesRepo.GetSubmitQuizCount(slide.Id, course.Id) / visitersCount) * 100)
							: 0,
					SuccessQuizPercentage = isQuiz ? userQuizzesRepo.GetAverageStatistics(slide.Id, course.Id) : 0,
					TotalHintCount = hintsCountOnSlide,
					HintUsedPercent = isExercise ? slideHintRepo.GetHintUsedPercent(slide.Id, course.Id, hintsCountOnSlide, db.Users.Count()) : 0
				});
			}
			return tableInfo;
		}

		public ActionResult UnitStatistics(UnitStatisticsParams param)
		{
			var courseId = param.CourseId;
			var unitName = param.UnitName;
			var periodStart = param.PeriodStartDate;
			var periodFinish = param.PeriodFinishDate;

			var realPeriodFinish = periodFinish.Add(TimeSpan.FromDays(1));

			var course = courseManager.GetCourse(courseId);
			var units = course.Slides.OrderBy(s => s.Index).Select(s => s.Info.UnitName).Distinct().ToList();
			if (string.IsNullOrEmpty(unitName) && units.Any())
				unitName = units.First();
			var slides = course.Slides.Where(s => s.Info.UnitName == unitName).ToList();
			var slidesIds = slides.Select(s => s.Id).ToImmutableHashSet();
			var quizzes = slides.OfType<QuizSlide>();
			var exersices = slides.OfType<ExerciseSlide>();

			var groups = groupsRepo.GetAvailableForUserGroups(courseId, User);
			var groupId = param.Group;
			List<string> filteredUsersIds = null;
			if (groupId.HasValue)
			{
				if (!groupsRepo.IsGroupAvailableForUser(groupId.Value, User))
					groupId = null;
				else
					filteredUsersIds = groupsRepo.GetGroupMembers(groupId.Value).Select(u => u.Id).ToList();
			}

			/* Dictionary<SlideId, List<Visit>> */
			var slidesVisits = visitsRepo.GetVisitsInPeriodForEachSlide(slidesIds, periodStart, realPeriodFinish, filteredUsersIds);

			var usersVisitedAllSlidesBeforePeriod = visitsRepo.GetUsersVisitedAllSlides(slidesIds, DateTime.MinValue, periodStart, filteredUsersIds);
			var usersVisitedAllSlidesInPeriod = visitsRepo.GetUsersVisitedAllSlides(slidesIds, periodStart, realPeriodFinish, filteredUsersIds);
			var usersVisitedAllSlidesBeforePeriodFinished = visitsRepo.GetUsersVisitedAllSlides(slidesIds, DateTime.MinValue, realPeriodFinish, filteredUsersIds);

			var quizzesAverageScore = quizzes.ToDictionary(q => q.Id,
				q => (int) slidesVisits.GetOrDefault(q.Id, new List<Visit>())
									   .Where(v => v.IsPassed)
									   .Select(v => 100 * Math.Min(v.Score, q.MaxScore) / q.MaxScore)
									   .DefaultIfEmpty(-1)
									   .Average()
			);

			/* Dictionary<SlideId, List<UserSolution> (distinct by user)> */
			var exercisesSolutions = userSolutionsRepo.GetAllAcceptedSolutions(courseId, slidesIds, periodStart, realPeriodFinish)
				.GroupBy(s => s.SlideId)
				.ToDictionary(g => g.Key, g => g.DistinctBy(s => s.UserId).ToList());

			var exercisesAcceptedSolutions = userSolutionsRepo.GetAllAcceptedSolutions(courseId, slidesIds, periodStart, realPeriodFinish)
				.GroupBy(s => s.SlideId)
				.ToDictionary(g => g.Key, g => g.DistinctBy(s => s.UserId).ToList());

			var manualQuizCheckQueueBySlide = slideCheckingsRepo.GetManualCheckingQueue<ManualQuizChecking>(
				new ManualCheckingQueueFilterOptions { CourseId = courseId, SlidesIds = slidesIds })
				.Where(i => periodStart <= i.Timestamp && i.Timestamp <= realPeriodFinish)
				.GroupBy(i => i.SlideId)
				.ToDictionary(g => g.Key, g => g.ToList());

			var commentsBySlide = commentsRepo.GetSlidesComments(courseId, slidesIds)
				.Where(c => periodStart <= c.PublishTime && c.PublishTime <= realPeriodFinish)
				.GroupBy(c => c.SlideId)
				.ToDictionary(g => g.Key, g => g.ToList());

			var visitedUsersIds = visitsRepo.GetVisitsInPeriod(slidesIds, periodStart, realPeriodFinish, filteredUsersIds)
				.ToList()
				.DistinctBy(v => v.UserId)
				.Select(v => v.User);

			var visitedSlidesByUser = visitsRepo.GetVisitsInPeriod(slidesIds, periodStart, realPeriodFinish, filteredUsersIds)
				.GroupBy(v => v.UserId)
				.ToDictionary(g => g.Key, g => g.Select(v => v.SlideId).ToImmutableHashSet());
			var visitedSlidesByUserAllTime = visitsRepo.GetVisitsInPeriod(slidesIds, DateTime.MinValue, DateTime.MaxValue, filteredUsersIds)
				.GroupBy(v => v.UserId)
				.ToDictionary(g => g.Key, g => g.Select(v => v.SlideId).ToImmutableHashSet());

			var model = new UnitStatisticPageModel
			{
				CourseId = courseId,
				UnitName = unitName,
				UnitsNames = units.ToList(),
				GroupId = groupId,
				Groups = groups,

				PeriodStart = periodStart,
				PeriodFinish = periodFinish,

				Slides = slides,
				SlidesVisits = slidesVisits,

				UsersVisitedAllSlidesBeforePeriod = usersVisitedAllSlidesBeforePeriod.ToList(),
				UsersVisitedAllSlidesInPeriod = usersVisitedAllSlidesInPeriod.ToList(),
				UsersVisitedAllSlidesBeforePeriodFinished = usersVisitedAllSlidesBeforePeriodFinished.ToList(),

				QuizzesAverageScore = quizzesAverageScore,
				ManualQuizCheckQueueBySlide = manualQuizCheckQueueBySlide,
				CommentsBySlide = commentsBySlide,
				ExercisesSolutions = exercisesSolutions,
				ExercisesAcceptedSolutions = exercisesAcceptedSolutions,
				VisitedUsersIds = visitedUsersIds.ToList(),
				VisitedSlidesByUser = visitedSlidesByUser,
				VisitedSlidesByUserAllTime = visitedSlidesByUserAllTime,
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
			IEnumerable<Slide> slides = null;
			if (courseId != null)
			{
				var course = courseManager.GetCourse(courseId);
				slides = course.Slides.Where(s => unitName == null || s.Info.UnitName == unitName);
			}
			var model = GetDailyStatistics(slides);
			return PartialView(model);
		}

		[ULearnAuthorize(Roles = LmsRoles.SysAdmin)]
		public ActionResult SystemStatistics()
		{
			return View();
		}

		public ActionResult UsersProgress(string courseId, string unitName, DateTime periodStart)
		{
			var course = courseManager.GetCourse(courseId);
			var slides = course.Slides
				.Where(s => s.Info.UnitName == unitName).ToArray();
			var users = GetUserInfos(slides, periodStart).OrderByDescending(GetRating).ToArray();
			return PartialView(new UserProgressViewModel
			{
				Slides = slides,
				Users = users,
				GroupsNames = groupsRepo.GetUsersGroupsNamesAsStrings(courseId, users.Select(u => u.UserId), User),
				CourseId = courseId
			});
		}

		private DailyStatistics[] GetDailyStatistics(IEnumerable<Slide> slides = null)
		{
			var slideIds = slides?.Select(s => s.Id).ToArray();
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
						TasksSolved = tasks.GetOrDefault(date, 0) + quizes.GetOrDefault(date, 0),
						SlidesVisited = visits.GetOrDefault(date, Tuple.Create(0, 0)).Item1,
						Score = visits.GetOrDefault(date, Tuple.Create(0, 0)).Item2
					})
					.ToArray();
			return result;
		}

		private Dictionary<DateTime, Tuple<int, int>> SumByDays(IQueryable<Visit> actions)
		{
			var q = from s in actions
					group s by DbFunctions.TruncateTime(s.Timestamp)
				into day
					select new { day.Key, sum = day.Sum(v => v.Score), count = day.Select(d => new { d.UserId, d.SlideId }).Distinct().Count() };
			return q.ToDictionary(d => d.Key.Value, d => Tuple.Create(d.count, d.sum));
		}

		private IQueryable<T> FilterBySlides<T>(IQueryable<T> source, IEnumerable<Guid> slideIds) where T : class, ISlideAction
		{
			return slideIds == null ? source : source.Where(s => slideIds.Contains(s.SlideId));
		}

		private IQueryable<T> FilterByTime<T>(IQueryable<T> source, DateTime firstDay, DateTime lastDay) where T : class, ISlideAction
		{
			return source.Where(s => s.Timestamp > firstDay && s.Timestamp <= lastDay);
		}

		private Dictionary<DateTime, int> GroupByDays<T>(IQueryable<T> actions) where T : class, ISlideAction
		{
			var q = from s in actions
					group s by DbFunctions.TruncateTime(s.Timestamp)
				into day
					select new { day.Key, count = day.Select(d => new { d.UserId, d.SlideId }).Distinct().Count() };
			return q.ToDictionary(d => d.Key.Value, d => d.count);
		}

		private Dictionary<DateTime, int> GetTasksSolvedStats(IEnumerable<Guid> slideIds, DateTime firstDay, DateTime lastDay)
		{
			return GroupByDays(FilterByTime(FilterBySlides(db.UserExerciseSubmissions, slideIds), firstDay, lastDay).Where(s => s.AutomaticChecking.IsRightAnswer));
		}

		private Dictionary<DateTime, int> GetQuizPassedStats(IEnumerable<Guid> slideIds, DateTime firstDay, DateTime lastDay)
		{
			return GroupByDays(FilterByTime(FilterBySlides(db.UserQuizzes, slideIds), firstDay, lastDay));
		}

		private Dictionary<DateTime, Tuple<int, int>> GetSlidesVisitedStats(IEnumerable<Guid> slideIds, DateTime firstDay, DateTime lastDay)
		{
			return SumByDays(FilterByTime(FilterBySlides(db.Visits, slideIds), firstDay, lastDay));
		}

		private SlideRateStats[] GetSlideRateStats(Course course, IEnumerable<Slide> slides)
		{
			var courseId = course.Id;
			var rates =
				(from rate in db.SlideRates
				 where rate.CourseId == courseId
				 group rate by new { rate.SlideId, rate.Rate }
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
						+ (s.IsQuizPassed ? s.QuizPercentage / 100.0 : 0));
		}

		private IEnumerable<UserInfo> GetUserInfos(Slide[] slides, DateTime periodStart, DateTime? periodFinish=null)
		{
			if (!periodFinish.HasValue)
				periodFinish = DateTime.Now;

			var slidesIds = slides.Select(s => s.Id).ToImmutableHashSet();

			var dq = visitsRepo.GetVisitsInPeriod(slidesIds, periodStart, periodFinish.Value)
				.Select(v => v.UserId)
				.Distinct()
				.Join(db.Visits, s => s, v => v.UserId, (s, visiters) => visiters)
				.Where(v => slidesIds.Contains(v.SlideId))
				.Select(v => new { v.UserId, v.User.UserName, v.SlideId, v.IsPassed, v.Score, v.AttemptsCount })
				.ToList();

			var r = dq.GroupBy(v => new { v.UserId, v.UserName }).Select(u => new UserInfo
			{
				UserId = u.Key.UserId,
				UserName = u.Key.UserName,
				SlidesSlideInfo = GetSlideInfo(slides, u.Select(arg => Tuple.Create(arg.SlideId, arg.IsPassed, arg.Score, arg.AttemptsCount)))
			});

			return r;
		}

		private static UserSlideInfo[] GetSlideInfo(IEnumerable<Slide> slides, IEnumerable<Tuple<Guid, bool, int, int>> slideResults)
		{
			var results = slideResults.GroupBy(tuple => tuple.Item1).ToDictionary(g => g.Key, g => g.First());
			var defaultValue = Tuple.Create(Guid.Empty, false, 0, 0);
			return slides
				.Select(slide => new
				{
					slide,
					result = results.GetOrDefault(slide.Id, defaultValue)
				})
				.Select(r => new UserSlideInfo
				{
					AttemptsCount = r.result.Item4,
					IsExerciseSolved = r.result.Item2,
					IsQuizPassed = r.result.Item2,
					QuizPercentage = r.slide is QuizSlide ? (double)r.result.Item3 / r.slide.MaxScore : 0.0,
					IsVisited = results.ContainsKey(r.slide.Id)
				})
				.ToArray();
		}

		public ActionResult ShowSolutions(string courseId, string userId, Guid slideId)
		{
			var solutions = db.UserExerciseSubmissions.Where(s => s.UserId == userId && s.SlideId == slideId).OrderByDescending(s => s.Timestamp).Take(10).ToList();
			var user = db.Users.Find(userId);
			var course = courseManager.GetCourse(courseId);
			var slide = (ExerciseSlide)course.FindSlideById(slideId);
			if (slide == null)
				return HttpNotFound($"No slide with id {slideId}");
			var model = new UserSolutionsViewModel
			{
				User = user,
				Course = course,
				GroupsNames = groupsRepo.GetUserGroupsNamesAsString(course.Id, userId, User),
				Slide = slide,
				Solutions = solutions
			};
			return View("UserSolutions", model);
		}
	}

	public class UnitStatisticsParams
	{
		public string CourseId { get; set; }
		public string UnitName { get; set; }
		
		public string PeriodStart { get; set; }
		public string PeriodFinish { get; set; }

		public int? Group { get; set; }

		private static readonly string[] dateFormats = { "dd.MM.yyyy" };

		public DateTime PeriodStartDate
		{
			get
			{
				var defaultPeriodStart = DateTime.Now.Subtract(TimeSpan.FromDays(7)).Date;
				if (string.IsNullOrEmpty(PeriodStart))
					return defaultPeriodStart;
				DateTime result;
				if (!DateTime.TryParseExact(PeriodStart, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
					return defaultPeriodStart;
				return result;
			}
		}

		public DateTime PeriodFinishDate
		{
			get
			{
				var defaultPeriodFinish = DateTime.Now.Date;
				if (string.IsNullOrEmpty(PeriodFinish))
					return defaultPeriodFinish;
				DateTime result;
				if (!DateTime.TryParseExact(PeriodFinish, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
					return defaultPeriodFinish;
				return result;
			}
		}
	}

	public class UserSolutionsViewModel
	{
		public ApplicationUser User { get; set; }
		public Course Course { get; set; }
		public string GroupsNames;
		public List<UserExerciseSubmission> Solutions { get; set; }
		public ExerciseSlide Slide { get; set; }
	}

	public class UserProgressViewModel
	{
		public string CourseId;
		public UserInfo[] Users;
		public Dictionary<string, string> GroupsNames;
		public Slide[] Slides;
	}
}