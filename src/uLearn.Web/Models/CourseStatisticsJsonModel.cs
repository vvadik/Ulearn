using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ApprovalUtilities.Utilities;
using uLearn;

namespace uLearn.Web.Models
{
	[DataContract]
	public class CourseStatisticsJsonModel
	{
		public CourseStatisticsJsonModel(CourseStatisticPageModel model)
		{
			Course = new CourseStatisticsJsonCourseInfo(model.Course);
			Groups = model.Groups.Select(g => new CourseStatisticsJsonGroupInfo(g)).ToArray();

			var students = new List<CourseStatisticsJsonStudentInfo>();
			var shouldBeSolvedSlides = model.Course.Slides.Where(s => s.ShouldBeSolved).ToList();
			foreach (var user in model.VisitedUsers)
			{
				var studentInfo = new CourseStatisticsJsonStudentInfo
				{
					UserId = user.UserId,
					VisibleName = user.UserVisibleName,
					GroupsIds = model.VisitedUsersGroups[user.UserId].ToArray(),
					SlidesScores = shouldBeSolvedSlides.Select(s => new CourseStatisticsJsonStudentSlideScore {
						SlideId = s.Id,
						Score = model.ScoreByUserAndSlide[Tuple.Create(user.UserId, s.Id)]
					}).ToArray(),
					AdditionalScores = model.Course.Units
						.SelectMany(
							u => u.Scoring.Groups.Values.Where(g => g.CanBeSetByInstructor).Select(g => new CourseStatisticsJsonStudentAdditionalScore
							{
								UnitId = u.Id,
								ScoringGroupId = g.Id,
								Score = model.AdditionalScores[Tuple.Create(user.UserId, u.Id, g.Id)],
							})
						)
						.ToArray()
				};
				students.Add(studentInfo);
			}
			Students = students.ToArray();
		}

		[DataMember(Name = "course")]
		public CourseStatisticsJsonCourseInfo Course;

		[DataMember(Name = "groups")]
		public CourseStatisticsJsonGroupInfo[] Groups;

		[DataMember(Name = "students")]
		public CourseStatisticsJsonStudentInfo[] Students;
	}

	[DataContract]
	public class CourseStatisticsJsonCourseInfo
	{
		public CourseStatisticsJsonCourseInfo(Course course)
		{
			Title = course.Title;
			Units = course.Units.Select(u => new CourseStatisticsJsonUnitInfo(u)).ToArray();
			ScoringGroups = course.Settings.Scoring.Groups.Values.Select(g => new CourseStatisticsJsonScoringGroupInfo(g)).ToArray();
		}

		[DataMember(Name = "title")]
		public string Title;

		[DataMember(Name = "units")]
		public CourseStatisticsJsonUnitInfo[] Units;

		[DataMember(Name = "scoring_groups")]
		public CourseStatisticsJsonScoringGroupInfo[] ScoringGroups;
	}

	[DataContract]
	public class CourseStatisticsJsonScoringGroupInfo
	{
		public CourseStatisticsJsonScoringGroupInfo(ScoringGroup group)
		{
			Id = group.Id;
			Abbreviation = group.Abbreviation;
			Name = group.Name;
		}

		[DataMember(Name = "id")]
		public string Id;

		[DataMember(Name = "abbreviation")]
		public string Abbreviation;

		[DataMember(Name = "name")]
		public string Name;
	}

	[DataContract]
	public class CourseStatisticsJsonUnitInfo
	{
		public CourseStatisticsJsonUnitInfo(Unit unit)
		{
			Id = unit.Id;
			Title = unit.Title;
			Slides = unit.Slides.Where(s => s.ShouldBeSolved).Select(s => new CourseStatisticsJsonSlideInfo(s)).ToArray();
			AdditionalScores = unit.Scoring.Groups.Values.Where(g => g.CanBeSetByInstructor).Select(g => new CourseStatisticsJsonUnitAdditionalScoreInfo(g)).ToArray();
		}

		[DataMember(Name = "id")]
		public Guid Id;

		[DataMember(Name = "title")]
		public string Title;

		[DataMember(Name = "slides")]
		public CourseStatisticsJsonSlideInfo[] Slides;

		[DataMember(Name = "additional_scores")]
		public CourseStatisticsJsonUnitAdditionalScoreInfo[] AdditionalScores;
	}

	[DataContract]
	public class CourseStatisticsJsonUnitAdditionalScoreInfo
	{
		public CourseStatisticsJsonUnitAdditionalScoreInfo(ScoringGroup g)
		{
			ScoringGroupId = g.Id;
			MaxAdditionalScore = g.MaxAdditionalScore;
		}

		[DataMember(Name = "scoring_group_id")]
		public string ScoringGroupId;

		[DataMember(Name = "max_additional_score")]
		public int MaxAdditionalScore;
	}

	[DataContract]
	public class CourseStatisticsJsonSlideInfo
	{
		public CourseStatisticsJsonSlideInfo(Slide s)
		{
			Id = s.Id;
			Title = s.Title;
			MaxScore = s.MaxScore;
		}

		[DataMember(Name = "id")]
		public Guid Id;

		[DataMember(Name = "title")]
		public string Title;

		[DataMember(Name = "max_score")]
		public int MaxScore;
	}

	[DataContract]
	public class CourseStatisticsJsonGroupInfo
	{
		public CourseStatisticsJsonGroupInfo(Group g)
		{
			Id = g.Id;
			Title = g.Name;
		}

		[DataMember(Name = "id")]
		public int Id;

		[DataMember(Name = "Title")]
		public string Title;
	}

	[DataContract]
	public class CourseStatisticsJsonStudentInfo
	{
		[DataMember(Name = "user_id")]
		public string UserId;

		[DataMember(Name = "groups")]
		public int[] GroupsIds;
	
		[DataMember(Name = "name")]
		public string VisibleName;

		[DataMember(Name = "slides_scores")]
		public CourseStatisticsJsonStudentSlideScore[] SlidesScores;

		[DataMember(Name = "additional_scores")]
		public CourseStatisticsJsonStudentAdditionalScore[] AdditionalScores;
	}

	[DataContract]
	public class CourseStatisticsJsonStudentSlideScore
	{
		[DataMember(Name = "slide_id")]
		public Guid SlideId;

		[DataMember(Name = "score")]
		public int Score;
	}

	[DataContract]
	public class CourseStatisticsJsonStudentAdditionalScore
	{
		[DataMember(Name = "unit_id")]
		public Guid UnitId;

		[DataMember(Name = "scoring_group_id")]
		public string ScoringGroupId;

		[DataMember(Name = "score")]
		public int Score;
	}
}