using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace uLearn.Web.Models
{
	public class UnitStatisticPageModel
	{
		public string CourseId { get; set; }
		public string UnitName { get; set; }
		public List<string> UnitsNames { get; set; }
		public int? GroupId { get; set; }
		public List<Group> Groups { get; set; }

		public DateTime PeriodStart { get; set; }
		public DateTime PeriodFinish { get; set; }

		public List<Slide> Slides { get; set; }
		public Dictionary<Guid, List<Visit>> SlidesVisits { get; set; }

		public List<string> UsersVisitedAllSlidesInPeriod { get; set; }
		public List<string> UsersVisitedAllSlidesBeforePeriod { get; set; }
		public List<string> UsersVisitedAllSlidesBeforePeriodFinished { get; set; }

		public Dictionary<Guid, int> QuizzesAverageScore { get; set; }
		public Dictionary<Guid, List<ManualQuizChecking>> ManualQuizCheckQueueBySlide { get; set; }
		public Dictionary<Guid, List<Comment>> CommentsBySlide { get; set; }

		public Dictionary<Guid, List<UserExerciseSubmission>> ExercisesSolutions { get; set; }
		public Dictionary<Guid, List<UserExerciseSubmission>> ExercisesAcceptedSolutions { get; set; }

		public List<ApplicationUser> VisitedUsersIds { get; set; }
		public Dictionary<string, ImmutableHashSet<Guid>> VisitedSlidesByUser { get; set; }
		public Dictionary<string, ImmutableHashSet<Guid>> VisitedSlidesByUserAllTime { get; set; }
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
