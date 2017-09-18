using System;
using System.Collections.Generic;
using System.Linq;
using Database.Models;
using uLearn.Extensions;

namespace uLearn.Web.Models
{
	public class StatisticPageModel
	{
		public Course Course { get; set; }
		public List<string> SelectedGroupsIds { get; set; }
		public string SelectedGroupsIdsJoined => string.Join(",", SelectedGroupsIds);
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
		public bool IsInstructor { get; set; }

		/* Dictionary<(userId, unitId, scoringGroupId), additionalScore> */
		public DefaultDictionary<Tuple<string, Guid, string>, int> AdditionalScores { get; set; }

		/* Dictionary<(unitId, scoringGroupId), List<Slide>> */
		public DefaultDictionary<Tuple<Guid, string>, List<Slide>> ShouldBeSolvedSlidesByUnitScoringGroup { get; set; }

		public SortedDictionary<string, ScoringGroup> ScoringGroups { get; set; }

		/* Dictionary<(userId, unitId, scoringGroupId), visitScore> */
		public DefaultDictionary<Tuple<string, Guid, string>, int> ScoreByUserUnitScoringGroup { get; set; }

		/* Dictionary<(userId, slideId), visitScore> */
		public DefaultDictionary<Tuple<string, Guid>, int> ScoreByUserAndSlide { get; set; }

		public DefaultDictionary<string, List<int>> VisitedUsersGroups { get; set; }

		public SortedDictionary<string, ScoringGroup> GetUsingUnitScoringGroups(Unit unit, SortedDictionary<string, ScoringGroup> courseScoringGroups)
		{
			return unit.Scoring.Groups
				.Where(kv => courseScoringGroups.ContainsKey(kv.Key))
				/* Filter out only scoring groups with should-be-solved slides or with additional scores by instructor */
				.Where(kv => ShouldBeSolvedSlidesByUnitScoringGroup[Tuple.Create(unit.Id, kv.Key)].Count > 0 || kv.Value.CanBeSetByInstructor)
				.ToDictionary(kv => kv.Key, kv => kv.Value)
				.ToSortedDictionary();
		}

		public int GetMaxScoreForUnitByScoringGroup(Unit unit, ScoringGroup scoringGroup)
		{
			var unitScoringGroup = unit.Scoring.Groups.Values.FirstOrDefault(g => g.Id == scoringGroup.Id);
			var maxAdditionalScore = unitScoringGroup != null && unitScoringGroup.CanBeSetByInstructor ? unitScoringGroup.MaxAdditionalScore : 0;
			return unit.Slides.Where(s => s.ScoringGroup == scoringGroup.Id).Sum(s => s.MaxScore) + maxAdditionalScore;
		}

		public int GetTotalScoreForUserInUnitByScoringGroup(string userId, Unit unit, ScoringGroup scoringGroup)
		{
			return ScoreByUserUnitScoringGroup[Tuple.Create(userId, unit.Id, scoringGroup.Id)] +
					AdditionalScores[Tuple.Create(userId, unit.Id, scoringGroup.Id)];
		}

		public int GetTotalOnlyFullScoreForUserInUnitByScoringGroup(string userId, Unit unit, ScoringGroup scoringGroup)
		{
			var shouldBeSolvedSlides = ShouldBeSolvedSlidesByUnitScoringGroup[Tuple.Create(unit.Id, scoringGroup.Id)];
			var onlyFullScore = 0;
			foreach (var slide in shouldBeSolvedSlides)
			{
				var slideScore = ScoreByUserAndSlide[Tuple.Create(userId, slide.Id)];
				onlyFullScore += GetOnlyFullScore(slideScore, slide.MaxScore);
			}
			return onlyFullScore + AdditionalScores[Tuple.Create(userId, unit.Id, scoringGroup.Id)];
		}

		public int GetOnlyFullScore(int score, int maxScore)
		{
			return score == maxScore ? score : 0;
		}
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
		public UnitStatisticUserInfo(ApplicationUser user)
		{
			UserId = user.Id;
			UserName = user.UserName;
			UserEmail = user.Email;
			UserVisibleName = user.VisibleName;
		}

		public string UserId { get; set; }
		public string UserVisibleName { get; set; }
		public string UserName { get; set; }
		public string UserEmail { get; set; }
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