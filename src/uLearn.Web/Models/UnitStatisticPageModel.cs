using System;
using System.Collections.Generic;

namespace uLearn.Web.Models
{
	public class StatisticPageModel
	{
		public Course Course { get; set; }
		public string GroupId { get; set; }
		public List<Group> Groups { get; set; }

		public DateTime PeriodStart { get; set; }
		public DateTime PeriodFinish { get; set; }

		public List<UnitStatisticUserInfo> VisitedUsers { get; set; }
		public bool VisitedUsersIsMore { get; set; }

		public Dictionary<string, List<int>> UsersGroupsIds { get; set; }
		public Dictionary<int, List<string>> EnabledAdditionalScoringGroupsForGroups { get; set; }
	}

	public class CourseStatisticPageModel : StatisticPageModel
	{
		/* Dictionary<(userId, unitId, scoringGroupId), additionalScore> */
		public DefaultDictionary<Tuple<string, Guid, string>, int> AdditionalScores { get; set; }

		/* Dictionary<(unitId, scoringGroupId), List<Slide>> */
		public DefaultDictionary<Tuple<Guid, string>, List<Slide>> ShouldBeSolvedSlidesByUnitScoringGroup { get; set; }

		public SortedDictionary<string, ScoringGroup> ScoringGroups { get; set; }
		/* Dictionary<(userId, unitId, scoringGroupId), visitScore> */
		public DefaultDictionary<Tuple<string, Guid, string>, int> ScoreByUserUnitScoringGroup { get; set; }
		/* Dictionary<(userId, slideId), visitScore> */
		public DefaultDictionary<Tuple<string, Guid>, int> ScoreByUserAndSlide { get; set; }
		public DefaultDictionary<string, string> VisiterUsersGroups { get; set; }
	}

	public class UnitStatisticPageModel : StatisticPageModel
	{
		public Unit Unit { get; set; }
		public List<Unit> Units { get; set; }

		public List<Slide> Slides { get; set; }
		public Dictionary<Guid, List<Visit>> SlidesVisits { get; set; }

		public int UsersVisitedAllSlidesInPeriodCount { get; set; }
		public int UsersVisitedAllSlidesBeforePeriodCount { get; set; }
		public int UsersVisitedAllSlidesBeforePeriodFinishedCount { get; set; }

		public Dictionary<Guid, int> QuizzesAverageScore { get; set; }

		public Dictionary<Guid, int> ExercisesSolutionsCount { get; set; }
		public Dictionary<Guid, int> ExercisesAcceptedSolutionsCount { get; set; }

		public Dictionary<string, int> VisitedSlidesCountByUser { get; set; }
		public Dictionary<string, int> VisitedSlidesCountByUserAllTime { get; set; }

		/* Dictionary<(userId, scoringGroupId), additionalScore> */
		public Dictionary<Tuple<string, string>, int> AdditionalScores { get; set; }
	}

	public class UnitStatisticUserInfo
	{
		public string UserId { get; set; }
		public string UserVisibleName { get; set; }
		public string UserName { get; set; }
	}

	public class DailyStatistics
	{
		public DateTime Day { get; set; }
		public int SlidesVisited { get; set; }
		public int TasksSolved { get; set; }
		public int QuizesPassed { get; set; }
		public int Score { get; set; }
	}

	public class SlideRateStats
	{
		public Guid SlideId { get; set; }
		public string SlideTitle { get; set; }
		public int NotUnderstand { get; set; }
		public int Good { get; set; }
		public int Trivial { get; set; }

	}

	public class UserInfo
	{
		public string UserId { get; set; }
		public string UserName { get; set; }
		public UserSlideInfo[] SlidesSlideInfo { get; set; }
	}

	public class UserSlideInfo
	{
		public bool IsVisited { get; set; }
		public bool IsExerciseSolved { get; set; }
		public int AttemptsCount { get; set; }
		public bool IsQuizPassed { get; set; }
		public double QuizPercentage { get; set; }
	}
}
