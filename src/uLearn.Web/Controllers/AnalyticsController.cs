using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
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
		private readonly VisitsRepo visitsRepo;
		private readonly UserSolutionsRepo userSolutionsRepo = new UserSolutionsRepo();
		private readonly GroupsRepo groupsRepo = new GroupsRepo();
		private readonly UsersRepo usersRepo = new UsersRepo();

		public AnalyticsController()
			: this(WebCourseManager.Instance)
		{
		}

		public AnalyticsController(CourseManager courseManager)
		{
			this.courseManager = courseManager;
			visitsRepo = new VisitsRepo(db);
		}

		public ActionResult UnitStatistics(UnitStatisticsParams param)
		{
			var courseId = param.CourseId;
			var unitId = param.UnitId;
			var periodStart = param.PeriodStartDate;
			var periodFinish = param.PeriodFinishDate;

			var realPeriodFinish = periodFinish.Add(TimeSpan.FromDays(1));

			var course = courseManager.GetCourse(courseId);
			if (! unitId.HasValue)
				return View("UnitStatisticsList", new UnitStatisticPageModel
				{
					CourseId = courseId,
					Units = course.Units,
				});
			var selectedUnit = course.GetUnitById(unitId.Value);
			var slides = selectedUnit.Slides;
			var slidesIds = slides.Select(s => s.Id).ToList();
			var quizzes = slides.OfType<QuizSlide>();
			var exersices = slides.OfType<ExerciseSlide>();

			var groups = groupsRepo.GetAvailableForUserGroups(courseId, User);
			var groupId = param.Group;
			var filterOptions = ControllerUtils.GetFilterOptionsByGroup<VisitsFilterOptions>(groupsRepo, User, courseId, groupId);
			filterOptions.SlidesIds = slidesIds;
			filterOptions.PeriodStart = periodStart;
			filterOptions.PeriodFinish = realPeriodFinish;

			/* Dictionary<SlideId, List<Visit>> */
			var slidesVisits = visitsRepo.GetVisitsInPeriodForEachSlide(filterOptions);

			var usersVisitedAllSlidesBeforePeriodCount = visitsRepo.GetUsersVisitedAllSlides(filterOptions.WithPeriodStart(DateTime.MinValue).WithPeriodFinish(periodStart)).Count();
			var usersVisitedAllSlidesInPeriodCount = visitsRepo.GetUsersVisitedAllSlides(filterOptions).Count();
			var usersVisitedAllSlidesBeforePeriodFinishedCount = visitsRepo.GetUsersVisitedAllSlides(filterOptions.WithPeriodStart(DateTime.MinValue)).Count();

			var quizzesAverageScore = quizzes.ToDictionary(q => q.Id,
				q => (int) slidesVisits.GetOrDefault(q.Id, new List<Visit>())
									   .Where(v => v.IsPassed)
									   .Select(v => 100 * Math.Min(v.Score, q.MaxScore) / q.MaxScore)
									   .DefaultIfEmpty(-1)
									   .Average()
			);

			/* Dictionary<SlideId, count (distinct by user)> */
			var exercisesSolutionsCount = userSolutionsRepo.GetAllSubmissions(courseId, slidesIds, periodStart, realPeriodFinish)
				.GroupBy(s => s.SlideId)
				.ToDictionary(g => g.Key, g => g.DistinctBy(s => s.UserId).Count());

			var exercisesAcceptedSolutionsCount = userSolutionsRepo.GetAllAcceptedSubmissions(courseId, slidesIds, periodStart, realPeriodFinish)
				.GroupBy(s => s.SlideId)
				.ToDictionary(g => g.Key, g => g.DistinctBy(s => s.UserId).Count());

			var visitedUsers = visitsRepo.GetVisitsInPeriod(filterOptions)
				.DistinctBy(v => v.UserId)
				.Join(db.Users, v => v.UserId, u => u.Id, (v, u) => new UnitStatisticUserInfo { UserId = u.Id, UserName = u.UserName, UserVisibleName = (u.LastName + u.FirstName != "" ? u.LastName + " " + u.FirstName : u.UserName).Trim() })
				.OrderBy(u => u.UserVisibleName)
				.ToList();

			var visitedSlidesCountByUser = visitsRepo.GetVisitsInPeriod(filterOptions)
				.GroupBy(v => v.UserId)
				.ToDictionary(g => g.Key, g => g.Count());
			var visitedSlidesCountByUserAllTime = visitsRepo.GetVisitsInPeriod(filterOptions.WithPeriodStart(DateTime.MinValue).WithPeriodFinish(DateTime.MaxValue))
				.GroupBy(v => v.UserId)
				.ToDictionary(g => g.Key, g => g.Count());

			var model = new UnitStatisticPageModel
			{
				CourseId = courseId,
				Units = course.Units,
				Unit = selectedUnit,
				GroupId = groupId,
				Groups = groups,

				PeriodStart = periodStart,
				PeriodFinish = periodFinish,

				Slides = slides,
				SlidesVisits = slidesVisits,

				UsersVisitedAllSlidesBeforePeriodCount = usersVisitedAllSlidesBeforePeriodCount,
				UsersVisitedAllSlidesInPeriodCount = usersVisitedAllSlidesInPeriodCount,
				UsersVisitedAllSlidesBeforePeriodFinishedCount = usersVisitedAllSlidesBeforePeriodFinishedCount,

				QuizzesAverageScore = quizzesAverageScore,
				ExercisesSolutionsCount = exercisesSolutionsCount,
				ExercisesAcceptedSolutionsCount = exercisesAcceptedSolutionsCount,
				VisitedUsers = visitedUsers,
				VisitedSlidesCountByUser = visitedSlidesCountByUser,
				VisitedSlidesCountByUserAllTime = visitedSlidesCountByUserAllTime,
			};
			return View(model);
		}

		public ActionResult UserUnitStatistics(string courseId, Guid unitId, string userId)
		{
			var course = courseManager.GetCourse(courseId);
			var user = usersRepo.FindUserById(userId);
			if (user == null)
				return HttpNotFound();

			var unit = course.GetUnitById(unitId);
			var slides = unit.Slides;
			var exercises = slides.OfType<ExerciseSlide>();
			var acceptedSubmissions = userSolutionsRepo
				.GetAllAcceptedSubmissionsByUser(courseId, exercises.Select(s => s.Id), userId)
				.OrderByDescending(s => s.Timestamp)
				.DistinctBy(u => u.SlideId);
			var userScores = visitsRepo.GetScoresForSlides(courseId, userId, slides.Select(s => s.Id));

			var model = new UserUnitStatisticsPageModel
			{
				Course = course,
				Unit = unit,
				User = user,
				Slides = slides.ToDictionary(s => s.Id),
				Submissions = acceptedSubmissions.ToList(),
				Scores = userScores,
			};

			return View(model);
		}

		public ActionResult SlideRatings(string courseId, Guid unitId)
		{
			var course = courseManager.GetCourse(courseId);
			var unit = course.GetUnitById(unitId);
			var slides = unit.Slides.ToArray();
			var model = GetSlideRateStats(course, slides);
			return PartialView(model);
		}

		public ActionResult DailyStatistics(string courseId, Guid? unitId)
		{
			IEnumerable<Slide> slides = null;
			if (courseId != null && unitId.HasValue)
			{
				var course = courseManager.GetCourse(courseId);

				var unit = course.GetUnitById(unitId.Value);
				slides = unit.Slides.ToArray();
			}
			var model = GetDailyStatistics(slides);
			return PartialView(model);
		}

		[ULearnAuthorize(Roles = LmsRoles.SysAdmin)]
		public ActionResult SystemStatistics()
		{
			return View();
		}

		public ActionResult UsersProgress(string courseId, Guid unitId, DateTime periodStart)
		{
			var course = courseManager.GetCourse(courseId);
			var unit = course.GetUnitById(unitId);
			var slides = unit.Slides.ToArray();
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
			return GroupByDays(FilterByTime(FilterBySlides(db.UserExerciseSubmissions, slideIds), firstDay, lastDay).Where(s => s.AutomaticCheckingIsRightAnswer));
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

		public ActionResult UserSolutions(string courseId, string userId, Guid slideId, int? version=null)
		{
			var user = db.Users.Find(userId);
			var course = courseManager.GetCourse(courseId);
			var slide = course.FindSlideById(slideId) as ExerciseSlide;
			if (slide == null)
				return RedirectToAction("CourseInfo", "Account", new {userName = userId, courseId});
			var model = new UserSolutionsViewModel
			{
				User = user,
				Course = course,
				GroupsNames = groupsRepo.GetUserGroupsNamesAsString(course.Id, userId, User),
				Slide = slide,
				SubmissionId = version
			};
			return View(model);
		}
	}

	public class UnitStatisticsParams
	{
		public string CourseId { get; set; }
		public Guid? UnitId { get; set; }
		
		public string PeriodStart { get; set; }
		public string PeriodFinish { get; set; }

		public string Group { get; set; }

		private static readonly string[] dateFormats = { "dd.MM.yyyy" };

		public DateTime PeriodStartDate
		{
			get
			{
				var defaultPeriodStart = GetDefaultPeriodStart();
				if (string.IsNullOrEmpty(PeriodStart))
					return defaultPeriodStart;
				DateTime result;
				if (!DateTime.TryParseExact(PeriodStart, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
					return defaultPeriodStart;
				return result;
			}
		}

		private static DateTime GetDefaultPeriodStart()
		{
			/* Select between January, 1 and September, 1 */
			var now = DateTime.Now;
			var periodStart = 1 < now.Month && now.Month < 9 ? new DateTime(now.Year, 1, 1) : new DateTime(now.Year, 9, 1);

			/* At least one month should be passed before now */
			var monthAgo = now.AddMonths(-1);
			if (periodStart > monthAgo)
				periodStart = monthAgo;
			return periodStart;
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
	
	public class UserUnitStatisticsPageModel
	{
		public Course Course { get; set; }
		public Unit Unit { get; set; }
		public ApplicationUser User { get; set; }
		public List<UserExerciseSubmission> Submissions { get; set; }
		public Dictionary<Guid, Slide> Slides { get; set; }
		public Dictionary<Guid, int> Scores { get; set; }
	}

	public class UserSolutionsViewModel
	{
		public ExerciseSlide Slide { get; set; }
		public ApplicationUser User { get; set; }
		public Course Course { get; set; }
		public string GroupsNames { get; set; }
		public int? SubmissionId { get; set; }
	}

	public class UserProgressViewModel
	{
		public string CourseId;
		public UserInfo[] Users;
		public Dictionary<string, string> GroupsNames;
		public Slide[] Slides;
	}
}